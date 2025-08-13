using Polly.CircuitBreaker;
using rinha_de_backend_2025.api.Configurations;
using rinha_de_backend_2025.api.Service;

namespace rinha_de_backend_2025.api.Infraestrutura.Workers;

internal sealed class CircuitBreakerHealthCheckWorker(
    ILogger<CircuitBreakerHealthCheckWorker> logger,
    PaymentResiliencePolicyProvider resiliencePolicyProvider, // Get the singleton provider
    IServiceScopeFactory scopeFactory)
    : BackgroundService
{
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Circuit Breaker Health Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Only perform a health check if the circuit is actually open
                if (resiliencePolicyProvider.BreakerPolicy.CircuitState == CircuitState.Open)
                {
                    logger.LogInformation("Circuit is open. Performing health check on PaymentProcessorDefault.");

                    using var scope = scopeFactory.CreateScope();
                    var paymentProcessor = scope.ServiceProvider.GetRequiredService<IPaymentProcessor>();
                    
                    var health = await paymentProcessor.PaymentProcessorDefaultIsHealthy();

                    if (health?.Failing == false)
                    {
                        logger.LogInformation("Health check successful. Manually resetting the circuit.");
                        resiliencePolicyProvider.BreakerPolicy.Reset();
                    }
                    else
                    {
                        logger.LogWarning("Health check failed. Circuit will remain open.");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in the Circuit Breaker Health Service.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        logger.LogInformation("Circuit Breaker Health Service is stopping.");
    }
}