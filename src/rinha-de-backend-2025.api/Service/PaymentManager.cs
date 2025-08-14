using Polly;
using Polly.Wrap;
using rinha_de_backend_2025.api.Configurations;
using rinha_de_backend_2025.api.Request;

namespace rinha_de_backend_2025.api.Service;

public class PaymentManager(
    IPaymentProcessor paymentProcessor,
    PaymentResiliencePolicyProvider resiliencePolicyProvider)
    : IPaymentManager
{
    private readonly AsyncPolicyWrap _resiliencePolicy = resiliencePolicyProvider.FallbackAndBreakerPolicy;

    public async Task SubmitPayment(PaymentRequest request, CancellationToken cancellationToken)
    {
        var context = new Context
        {
            ["request"] = request,
            ["paymentProcessor"] = paymentProcessor
        };

        await _resiliencePolicy.ExecuteAsync(async _ => await Execute(request), context);
    }

    private async Task Execute(PaymentRequest request)
    {
        await paymentProcessor.PaymentProcessorDefault(request);
    }
}