using rinha_de_backend_2025.api.Request;

namespace rinha_de_backend_2025.api.Service;

public interface IPaymentManager
{
    Task SubmitPayment(PaymentRequest request, CancellationToken cancellationToken);
}