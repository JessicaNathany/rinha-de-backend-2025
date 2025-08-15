using System.Net;
using rinha_de_backend_2025.api.Entity;
using rinha_de_backend_2025.api.Request;

namespace rinha_de_backend_2025.api.Infraestrutura.Clients;

public class PaymentGatewayClient(IHttpClientFactory httpClientFactory, ILogger<PaymentGatewayClient> logger)
    : IPaymentGatewayClient
{
    public async Task<Payments> ProcessPaymentDefault(PaymentRequest request)
    {
        return await ProcessPayment(request, service_used.Default);
    }

    public async Task<Payments> ProcessPaymentFallback(PaymentRequest request)
    {
        return await ProcessPayment(request, service_used.Fallback);
    }
    
    public async Task<ServiceHealthStatus?> GetProcessPaymentDefaultHealthStatus()
    {
        var client = httpClientFactory.CreateClient(nameof(service_used.Default));
        
        return await client.GetFromJsonAsync<ServiceHealthStatus>("/payments/service-health");
    }
    
    private async Task<Payments> ProcessPayment(PaymentRequest request, service_used serviceType)
    {
        var client = httpClientFactory.CreateClient(serviceType.ToString());
        var requestTime = DateTime.UtcNow;

        try
        {
            var requestPayload = new
            {
                correlationId = request.CorrelationId,
                amount = request.Amount,
                requestedAt = requestTime.ToString("o")
            };

            logger.LogInformation("Processing payment with {ClientName} for correlation {CorrelationId}", 
                serviceType.ToString(), request.CorrelationId);

            var response = await client.PostAsJsonAsync("", requestPayload);

            return response.StatusCode switch
            {
                HttpStatusCode.OK => new Payments
                {
                    correlation_id = Guid.Parse(request.CorrelationId),
                    amount = request.Amount,
                    service_used = serviceType,
                    requested_at = requestTime
                },
                HttpStatusCode.BadRequest => throw new BadHttpRequestException(
                    $"Bad request when processing payment with {serviceType.ToString()}", 
                    (int)HttpStatusCode.BadRequest),
                _ => throw new HttpRequestException(
                    $"Payment processing failed with status {response.StatusCode}: {response.ReasonPhrase}")
            };
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP error occurred while processing payment with {ClientName} for correlation {CorrelationId}", 
                serviceType.ToString(), request.CorrelationId);
            throw new InvalidOperationException(
                $"Payment processing failed due to network error with {serviceType.ToString()}", ex);
        }
        catch (TaskCanceledException ex)
        {
            logger.LogError(ex, "Timeout occurred while processing payment with {ClientName} for correlation {CorrelationId}", 
                serviceType.ToString(), request.CorrelationId);
            throw new TimeoutException(
                $"Payment processing timed out with {serviceType.ToString()}", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred while processing payment with {ClientName} for correlation {CorrelationId}", 
                serviceType.ToString(), request.CorrelationId);
            throw new InvalidOperationException(
                $"Unexpected error occurred while processing the payment request with {serviceType.ToString()}", ex);
        }
    }
}