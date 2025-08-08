using Npgsql;
using System.Data;

namespace rinha_de_backend_2025.api.Infraestrutura.Postgres
{
    public class PostgresConnection : IPostgresConnection
    {
        private readonly string _connectionString;
        public PostgresConnection(IConfiguration configuration)
        {
            _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        }
        public async Task<IDbConnection> OpenConnectionAsync()
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            
            return connection;
        }
    }
}
