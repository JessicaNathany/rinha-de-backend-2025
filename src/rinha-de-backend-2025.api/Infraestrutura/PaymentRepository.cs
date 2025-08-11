using Dapper;
using rinha_de_backend_2025.api.Entity;
using rinha_de_backend_2025.api.Infraestrutura.Postgres;
using static Dapper.SqlMapper;

namespace rinha_de_backend_2025.api.Infraestrutura
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IPostgresConnection _postgresConnection;

        public PaymentRepository(IPostgresConnection postgresConnection)
        {
            _postgresConnection = postgresConnection;
        }

        public async Task<List<Payments>> Get()
        {
            try
            {
                using (var connection = await _postgresConnection.OpenConnectionAsync())
                {
                    var query = "@select count(1) as request," +
                                "sum(amount), as amount, service_used from payments group by service_used";

                    var result = await connection.QueryAsync<Payments>(query);
                    return result.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Payments> Save(Payments entity)
        {
            try
            {
                using (var connection = await _postgresConnection.OpenConnectionAsync())
                {
                    var query = "@insert into payments (correlation_id, amount, requested_at, service_used)" +
                                "values (@CorrelationId, @Amount, @RequestedAt, @ServiceType)";

                    await connection.ExecuteAsync(query, new
        {
                        entity.CorrelationId,
                        entity.Amount,
                        ServiceType = entity.ServiceType.ToString(),
                        entity.RequestedAt
                    });
            
                    var result = await connection.QueryFirstAsync<Payments>(query);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
