using System.Data;

namespace rinha_de_backend_2025.api.Infraestrutura.Postgres
{
    public interface IPostgresConnection
    {
        Task<IDbConnection> OpenConnectionAsync();
    }
}
