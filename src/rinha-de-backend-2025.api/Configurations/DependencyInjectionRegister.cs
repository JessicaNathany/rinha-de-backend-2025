using rinha_de_backend_2025.api.Service;

namespace rinha_de_backend_2025.api.Configurations
{
    public static class DependencyInjectionRegister
    {
        public static void ResolveDependencies(this IServiceCollection service)
        {
            service.AddScoped<IPaymentProcessor, PaymentProcessor>();
        }
    }
}
