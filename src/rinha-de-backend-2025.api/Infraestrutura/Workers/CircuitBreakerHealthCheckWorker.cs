using Polly.CircuitBreaker;
using rinha_de_backend_2025.api.Configurations;
using rinha_de_backend_2025.api.Infraestrutura.Clients;
using rinha_de_backend_2025.api.Infraestrutura.Repositories;

namespace rinha_de_backend_2025.api.Infraestrutura.Workers;

internal sealed class CircuitBreakerHealthCheckWorker(
    ILogger<CircuitBreakerHealthCheckWorker> logger,
    PaymentResiliencePolicyProvider resiliencePolicyProvider, // Get the singleton provider
    IServiceScopeFactory scopeFactory)
    : BackgroundService
{
    private readonly string _instanceName = Environment.GetEnvironmentVariable("INSTANCE_NAME") ?? PrimaryServiceInstanceName;

    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(5);
    private readonly TimeSpan _cacheCheckInterval = TimeSpan.FromSeconds(2);
    private const string PrimaryServiceInstanceName = "backend-api-1";
    private const string PaymentProcessorDefault = "PaymentProcessorDefault";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Circuit Breaker Health Service starting with instance ID: {InstanceId}", _instanceName);

        var healthCheckTask = PerformHealthChecksAsync(stoppingToken);
        var cacheMonitorTask = MonitorCacheAsync(stoppingToken);

        await Task.WhenAny(healthCheckTask, cacheMonitorTask);

        logger.LogInformation("Circuit Breaker Health Service is stopping.");
    }

    private async Task PerformHealthChecksAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (resiliencePolicyProvider.BreakerPolicy.CircuitState == CircuitState.Open)
                {
                    if (TryAcquireLock())
                    {
                        logger.LogInformation("Acquired lock. Performing health check on PaymentProcessorDefault");
                        await VerifyHealthCheckFromExternalService();
                    }
                    else
                    {
                        logger.LogDebug("Could not acquire lock. Another instance is performing the health check");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in health check loop");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private bool TryAcquireLock()
    {
        return _instanceName == PrimaryServiceInstanceName;
    }

    private async Task VerifyHealthCheckFromExternalService()
    {
        using var scope = scopeFactory.CreateScope();
        var paymentGatewayClient = scope.ServiceProvider.GetRequiredService<IPaymentGatewayClient>();
        var cacheRepository = scope.ServiceProvider.GetRequiredService<IHealthCheckCacheRepository>();

        try
        {
            logger.LogInformation("Circuit is open. Performing health check on PaymentProcessorDefault.");

            var health = await paymentGatewayClient.GetProcessPaymentDefaultHealthStatus(PaymentProcessorDefault, _instanceName);
            if (health == null)
                return;
            
            await cacheRepository.SaveAsync(health);

            if (health.IsHealthyAndMinResponseTimeIsOk())
            {
                logger.LogInformation("Health check successful. Manually resetting the circuit.");
                resiliencePolicyProvider.BreakerPolicy.Reset();
            }
            else
            {
                logger.LogWarning("Health check failed. Circuit will remain open.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred in the Circuit Breaker Health Service.");
        }
    }

    private async Task MonitorCacheAsync(CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();
        var cacheRepository = scope.ServiceProvider.GetRequiredService<IHealthCheckCacheRepository>();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (resiliencePolicyProvider.BreakerPolicy.CircuitState == CircuitState.Open)
                {
                    var cachedStatus = await cacheRepository.GetHealthStatusAsync(PaymentProcessorDefault);

                    if (cachedStatus != null)
                    {
                        var cacheAge = DateTime.UtcNow - cachedStatus.LastChecked;
                        if (cacheAge <= TimeSpan.FromSeconds(6))
                        {
                            if (cachedStatus.IsHealthyAndMinResponseTimeIsOk())
                            {
                                logger.LogInformation(
                                    "Cache indicates service is healthy (checked {Age:F1}s ago by {CheckedBy}). Resetting circuit",
                                    cacheAge.TotalSeconds,
                                    cachedStatus.CheckedBy);

                                resiliencePolicyProvider.BreakerPolicy.Reset();
                            }
                            else
                            {
                                logger.LogDebug(
                                    "Cache indicates service is unhealthy (checked {Age:F1}s ago). Circuit remains open",
                                    cacheAge.TotalSeconds);
                            }
                        }
                        else
                        {
                            logger.LogWarning(
                                "Cached health status is stale ({Age:F1}s old). Ignoring cache",
                                cacheAge.TotalSeconds);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error monitoring health check cache");
            }

            await Task.Delay(_cacheCheckInterval, stoppingToken);
        }
    }
}