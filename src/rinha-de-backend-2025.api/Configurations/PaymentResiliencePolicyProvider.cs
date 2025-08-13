using Polly;
using Polly.CircuitBreaker;
using Polly.Wrap;
using rinha_de_backend_2025.api.Request;
using rinha_de_backend_2025.api.Service;

namespace rinha_de_backend_2025.api.Configurations;

public class PaymentResiliencePolicyProvider
{
    public AsyncCircuitBreakerPolicy BreakerPolicy { get; }

    public AsyncPolicyWrap FallbackAndBreakerPolicy { get; }

    public PaymentResiliencePolicyProvider(ILogger<PaymentResiliencePolicyProvider> logger)
    {
        var fallbackPolicy = Policy
            .Handle<Exception>() // Catches any failure from the inner policy, including BrokenCircuitException
            .FallbackAsync(
                fallbackAction: async (context, cancellationToken) =>
                {
                    logger.LogWarning("Circuit is open or initial request failed. Executing fallback.");
                    if (!context.TryGetValue("paymentProcessor", out var processorObj) ||
                        processorObj is not IPaymentProcessor paymentProcessor)
                        throw new InvalidOperationException("Missing 'paymentProcessor' in Polly context.");
                    
                    var request = (PaymentRequest)context["request"];
                    _ = await paymentProcessor.PaymentProcessorFallback(request);

                },
                onFallbackAsync: (exception, context) =>
                {
                    logger.LogWarning(exception, "Fallback initiated. Reason: {Reason}", exception.Message);
                    return Task.CompletedTask;
                }
            );

        BreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 1,
                durationOfBreak: TimeSpan.FromMinutes(60),
                onBreak: (ex, breakDelay) => logger.LogWarning(ex, "Circuit broken for {BreakDelay}.", breakDelay),
                onReset: () => logger.LogInformation("Circuit has been manually reset by the health service."),
                onHalfOpen: () => { }
            );

        FallbackAndBreakerPolicy = Policy.WrapAsync(fallbackPolicy, BreakerPolicy);
    }
}
