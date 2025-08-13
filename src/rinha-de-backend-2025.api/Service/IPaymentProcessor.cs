using rinha_de_backend_2025.api.Entity;
using rinha_de_backend_2025.api.Request;
using rinha_de_backend_2025.api.Response;

namespace rinha_de_backend_2025.api.Service
{
    public interface IPaymentProcessor
    {
        Task<Payments> PaymentProcessorDefault(PaymentRequest request);
        Task<Payments> PaymentProcessorFallback(PaymentRequest request);
        Task<List<Payments>> GetPaymentSummary();
        Task<bool> PaymentProcessorDefaultIsHealthy();
    }
}
