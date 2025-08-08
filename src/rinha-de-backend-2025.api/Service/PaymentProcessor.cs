using rinha_de_backend_2025.api.Entity;
using rinha_de_backend_2025.api.Infraestrutura;
using rinha_de_backend_2025.api.Request;
using rinha_de_backend_2025.api.Response;
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
        public async Task PaymentProcessorDefault(PaymentRequest request)
        {
            var httpClient = new HttpClient();
            var url = Environment.GetEnvironmentVariable("PAYMENT_DEFAULT_URL");

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
                RequestedAt = DateTime.UtcNow
            };

            switch (responseMessage.StatusCode)
            {
                case HttpStatusCode.OK:
                    await _paymentRepository.Save(payments);
                    return;
                case HttpStatusCode.BadRequest:
                    throw new BadHttpRequestException("Bad Request", (int)HttpStatusCode.BadRequest);
                case HttpStatusCode.NotFound:
                    throw new BadHttpRequestException("Not found", (int)HttpStatusCode.NotFound);
                case HttpStatusCode.InternalServerError:
                    throw new BadHttpRequestException("Internal Server Error", (int)HttpStatusCode.InternalServerError);
                default:
                    throw new Exception("Unexpected error occurred while processing the payment request.");
            }
        }

        public Task<FallbackResponse> PaymentProcessorFallback(PaymentRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
