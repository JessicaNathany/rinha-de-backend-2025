using rinha_de_backend_2025.api.Entity;
using rinha_de_backend_2025.api.Infraestrutura;
using rinha_de_backend_2025.api.Request;
using System.Net;
using System.Text;
using System.Text.Json;

namespace rinha_de_backend_2025.api.Service
{
    public class PaymentProcessor : IPaymentProcessor
    {
        private readonly IPaymentRepository _paymentRepository;
        public PaymentProcessor(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository; 
        }
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
                CorrelationId = Guid.Parse(request.CorrelationId),
                Amount = request.Amount,   
                ServiceType = ServiceType.Default,
                RequestedAt = DateTime.UtcNow
            };

            switch (responseMessage.StatusCode)
            {
                case HttpStatusCode.OK:
                    return await _paymentRepository.Save(payments);
                case HttpStatusCode.BadRequest:
                    throw new BadHttpRequestException("Bad Request", (int)HttpStatusCode.BadRequest);
                case HttpStatusCode.InternalServerError:
                    throw new BadHttpRequestException("Internal Server Error", (int)HttpStatusCode.InternalServerError);
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
                CorrelationId = Guid.Parse(request.CorrelationId),
                Amount = request.Amount,   
                ServiceType = ServiceType.Default,
                RequestedAt = DateTime.UtcNow
            };

            switch (responseMessage.StatusCode)
            {
                case HttpStatusCode.OK:
                    return await _paymentRepository.Save(payments);
                case HttpStatusCode.BadRequest:
                    throw new BadHttpRequestException("Bad Request", (int)HttpStatusCode.BadRequest);
                case HttpStatusCode.InternalServerError:
                    throw new BadHttpRequestException("Internal Server Error", (int)HttpStatusCode.InternalServerError);
                default:
                    throw new Exception("Unexpected error occurred while processing the payment request.");
            }
        }

        public async Task<List<Payments>> GetPaymentSummary()
        {
            return await _paymentRepository.Get();
        }
    }
}
