using Dapper;
using rinha_de_backend_2025.api.Entity;
using rinha_de_backend_2025.api.Infraestrutura.Postgres;

namespace rinha_de_backend_2025.api.Infraestrutura.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IPostgresConnection _postgresConnection;

        public PaymentRepository(IPostgresConnection postgresConnection)
        {
            _postgresConnection = postgresConnection;
        }

        public async Task<List<PaymentSummary>> GetPaymentSummary(DateTime? startDate, DateTime? endDate)
        {
            using (var connection = await _postgresConnection.OpenConnectionAsync())
            {
                var conditions = new List<string>();
                var parameters = new DynamicParameters();

                if (startDate.HasValue)
                {
                    conditions.Add("requested_at >= @startDate");
                    parameters.Add("startDate", startDate.Value);
                }

                if (endDate.HasValue)
                {
                    conditions.Add("requested_at <= @endDate");
                    parameters.Add("endDate", endDate.Value);
                }
                
                var whereClause = conditions.Any() ? "WHERE " + string.Join(" AND ", conditions) : "";

                var query = $@"SELECT
                          count(1) as Request,
                          sum(amount) as Amount,
                          service_used as ServiceUsed
                       FROM payments
                       {whereClause}
                       GROUP BY service_used";
                
                var result = await connection.QueryAsync<PaymentSummary>(query, new { startDate, endDate });
                    
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
