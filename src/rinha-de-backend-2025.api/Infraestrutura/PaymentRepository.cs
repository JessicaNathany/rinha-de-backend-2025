using Dapper;
using rinha_de_backend_2025.api.Entity;
using rinha_de_backend_2025.api.Infraestrutura.Postgres;
using rinha_de_backend_2025.api.Response;

namespace rinha_de_backend_2025.api.Infraestrutura
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IPostgresConnection _postgresConnection;

        public PaymentRepository(IPostgresConnection postgresConnection)
        {
            _postgresConnection = postgresConnection;
        }

        public async Task<List<PaymentSummary>> GetPaymentSummary()
        {
            using (var connection = await _postgresConnection.OpenConnectionAsync())
            {
                var query = @"select
                                     count(1) as Request,
                                     sum(amount) as Amount,
                                     service_used as ServiceUsed
                                  from payments
                                  group by service_used";

                var result = await connection.QueryAsync<PaymentSummary>(query);
                    
                return result.ToList();
            }
        }

        public async Task<List<Payments>> GetAll()
        {
            using (var connection = await _postgresConnection.OpenConnectionAsync())
            {
                var query = @"select * from payments";

                var result = await connection.QueryAsync<Payments>(query);

                return result.ToList();
            }
        }

        public async Task<Payments> Save(Payments entity)
        {
            using (var connection = await _postgresConnection.OpenConnectionAsync())
            {
                var query = @"insert into payments (correlation_id, amount, requested_at, service_used)
                          values (@correlation_id, @amount, @requested_at, @service_used)
                          returning *;";

                var result = await connection.QueryFirstAsync<Payments>(query, new
                {
                    entity.correlation_id,
                    entity.amount,
                    service_used = (int)entity.service_used,
                    entity.requested_at
                });

                return result;
            }
        }
    }
}
