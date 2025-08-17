using rinha_de_backend_2025.api.Entity;

namespace rinha_de_backend_2025.api.Infraestrutura.Repositories;

public interface IHealthCheckCacheRepository
{
    Task<HealthCheckStatus?> GetHealthStatusAsync(string serviceName);
    Task SaveAsync(HealthCheckStatus status);
}