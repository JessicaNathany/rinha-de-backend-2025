using rinha_de_backend_2025.api.Entity;
using rinha_de_backend_2025.api.Infraestrutura;
using rinha_de_backend_2025.api.Request;
using System.Net;
using System.Text;
using System.Text.Json;

namespace rinha_de_backend_2025.api.Service;

public class PaymentProcessor(IPaymentRepository paymentRepository) : IPaymentProcessor
{
    public async Task<Payments> PaymentProcessorDefault(PaymentRequest request)
    {
        var httpClient = new HttpClient();
        var url = Environment.GetEnvironmentVariable("PAYMENT_PROCESSOR_DEFAULT");

        var body = new
        {
            correlationId = request.CorrelationId,
            amount = request.Amount,
            requestedAt = DateTime.UtcNow.ToString("o")
        };

        var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        var responseMessage = await httpClient.PostAsync(url, content);

        var payments = new Payments
        {
            correlation_id = Guid.Parse(request.CorrelationId),
            amount = request.Amount,
            service_used = service_used.Default,
            requested_at = DateTime.UtcNow
        };

        switch (responseMessage.StatusCode)
        {
            case HttpStatusCode.OK:
                return await paymentRepository.Save(payments);
            case HttpStatusCode.BadRequest:
                throw new BadHttpRequestException("Bad Request", (int)HttpStatusCode.BadRequest);
            default:
                throw new Exception("Unexpected error occurred while processing the payment request.");
        }
    }

    public async Task<Payments> PaymentProcessorFallback(PaymentRequest request)
    {
        var httpClient = new HttpClient();
        var url = Environment.GetEnvironmentVariable("PAYMENT_PROCESSOR_FALLBACK");

        var body = new
        {
            correlationId = request.CorrelationId,
            amount = request.Amount,
            requestedAt = DateTime.UtcNow.ToString("o")
        };

        var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        var responseMessage = await httpClient.PostAsync(url, content);

        var payments = new Payments
        {
            correlation_id = Guid.Parse(request.CorrelationId),
            amount = request.Amount,
            service_used = service_used.Default,
            requested_at = DateTime.UtcNow
        };

        switch (responseMessage.StatusCode)
        {
            case HttpStatusCode.OK:
                return await paymentRepository.Save(payments);
            case HttpStatusCode.BadRequest:
                throw new BadHttpRequestException("Bad Request", (int)HttpStatusCode.BadRequest);
            default:
                throw new Exception("Unexpected error occurred while processing the payment request.");
        }
    }

    public async Task<List<Payments>> GetPaymentSummary()
    {
        return await paymentRepository.Get();
    }

    public async Task<ServiceHealthStatus?> PaymentProcessorDefaultIsHealthy()
    {
        using var httpClient = new HttpClient();
        var url = Environment.GetEnvironmentVariable("PAYMENT_PROCESSOR_DEFAULT") + "/service-health";
        return await httpClient.GetFromJsonAsync<ServiceHealthStatus>(url);
    }
}