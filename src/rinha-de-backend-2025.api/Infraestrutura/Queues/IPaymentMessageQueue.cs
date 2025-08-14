using rinha_de_backend_2025.api.Request;

namespace rinha_de_backend_2025.api.Infraestrutura.Queues;

public interface IPaymentMessageQueue
{
    Task PublishAsync(PaymentRequest message, CancellationToken cancellationToken = default);
    IAsyncEnumerable<PaymentRequest> ConsumeAsync(CancellationToken cancellationToken = default);
}