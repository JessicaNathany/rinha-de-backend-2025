using rinha_de_backend_2025.api.Infraestrutura.Clients;
using rinha_de_backend_2025.api.Infraestrutura.Postgres;
using rinha_de_backend_2025.api.Infraestrutura.Queues;
using rinha_de_backend_2025.api.Infraestrutura.Repositories;
using rinha_de_backend_2025.api.Service;

namespace rinha_de_backend_2025.api.Configurations
{
    public static class DependencyInjectionRegister
    {
        public static void ResolveDependencies(this IServiceCollection service)
        {
            service.AddSingleton<PaymentResiliencePolicyProvider>();
            service.AddSingleton<IPaymentMessageQueue, PaymentMessageQueue>();
            service.AddScoped<IPaymentManager, PaymentManager>();
            service.AddScoped<IPaymentProcessor, PaymentProcessor>();
            service.AddScoped<IPostgresConnection, PostgresConnection>();
            service.AddScoped<IPaymentRepository, PaymentRepository>();
            service.AddScoped<IHealthCheckCacheRepository, HealthCheckCacheRepository>();
            service.AddSingleton<IPaymentGatewayClient, PaymentGatewayClient>();
        }
    }
}
