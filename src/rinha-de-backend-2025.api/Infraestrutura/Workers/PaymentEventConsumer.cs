using rinha_de_backend_2025.api.Infraestrutura.Queues;
using rinha_de_backend_2025.api.Service;

namespace rinha_de_backend_2025.api.Infraestrutura.Workers;

internal sealed class PaymentEventConsumer(IPaymentMessageQueue queue, ILogger<PaymentEventConsumer> logger, IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in queue.ConsumeAsync(stoppingToken))
        {
            logger.LogInformation("Payment request consumed: {Message}", message);
            
            using var scope = scopeFactory.CreateScope();
            
            var paymentManager = scope.ServiceProvider.GetRequiredService<IPaymentManager>();
            
            await paymentManager.SubmitPayment(message, stoppingToken);
        }
    }
}