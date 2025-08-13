using rinha_de_backend_2025.api.Infraestrutura.Queues;

namespace rinha_de_backend_2025.api.Infraestrutura.Workers;

internal sealed class PaymentEventConsumer(IPaymentMessageQueue queue, ILogger<PaymentEventConsumer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in queue.ConsumeAsync(stoppingToken))
        {
            logger.LogInformation("Payment request consumed: {Message}", message);
        }
    }
}