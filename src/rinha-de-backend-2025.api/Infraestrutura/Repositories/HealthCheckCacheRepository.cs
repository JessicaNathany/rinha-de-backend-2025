using Dapper;
using rinha_de_backend_2025.api.Entity;
using rinha_de_backend_2025.api.Infraestrutura.Postgres;

namespace rinha_de_backend_2025.api.Infraestrutura.Repositories;

public class HealthCheckCacheRepository(IPostgresConnection postgresConnection)
    : IHealthCheckCacheRepository
{
    public async Task<HealthCheckStatus?> GetHealthStatusAsync(string serviceName)
    {
        const string sql = """

                                       SELECT service_name as ServiceName, 
                                              is_healthy as IsHealthy, 
                                              min_response_time as MinResponseTime, 
                                              last_checked as LastChecked, 
                                              checked_by as CheckedBy
                                       FROM health_check_cache 
                                       WHERE service_name = @serviceName
                           """;

        using var connection = await postgresConnection.OpenConnectionAsync();

        return await connection.QueryFirstOrDefaultAsync<HealthCheckStatus>(sql, new { serviceName });
    }

    public async Task SaveAsync(HealthCheckStatus status)
    {
        const string sql = """

                                       INSERT INTO health_check_cache (service_name, is_healthy, min_response_time, last_checked, checked_by)
                                       VALUES (@serviceName, @isHealthy, @minResponseTime, @lastChecked, @checkedBy)
                                       ON CONFLICT (service_name) 
                                       DO UPDATE SET 
                                           is_healthy = EXCLUDED.is_healthy,
                                           min_response_time = EXCLUDED.min_response_time,
                                           last_checked = EXCLUDED.last_checked,
                                           checked_by = EXCLUDED.checked_by
                           """;

        using var connection = await postgresConnection.OpenConnectionAsync();
        await connection.ExecuteAsync(sql, new
        {
            serviceName = status.ServiceName,
            isHealthy = status.IsHealthy,
            minResponseTime = status.MinResponseTime,
            lastChecked = status.LastChecked,
            checkedBy = status.CheckedBy
        });
    }
}