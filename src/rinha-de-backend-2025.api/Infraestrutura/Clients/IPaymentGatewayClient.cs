using rinha_de_backend_2025.api.Entity;
using rinha_de_backend_2025.api.Request;

namespace rinha_de_backend_2025.api.Infraestrutura.Clients;

public interface IPaymentGatewayClient
{
    Task<Payments> ProcessPaymentDefault(PaymentRequest request);
    Task<Payments> ProcessPaymentFallback(PaymentRequest request);
    Task<ServiceHealthStatus?> GetProcessPaymentDefaultHealthStatus();
}