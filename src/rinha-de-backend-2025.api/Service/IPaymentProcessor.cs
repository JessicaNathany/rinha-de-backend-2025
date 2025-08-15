using rinha_de_backend_2025.api.Entity;
using rinha_de_backend_2025.api.Request;

namespace rinha_de_backend_2025.api.Service;

public interface IPaymentProcessor
{
    Task<Payments> PaymentProcessorDefault(PaymentRequest request);
    Task<Payments> PaymentProcessorFallback(PaymentRequest request);
    Task<(List<Payments> paymentSummary, List<Payments> payments)> GetSummaryAndAll();
}