using rinha_de_backend_2025.api.Entity;

namespace rinha_de_backend_2025.api.Infraestrutura
{
    public interface IPaymentRepository
    {
        Task<Payments> Save(Payments entity);
        Task<List<Payments>> Get();
    }
}
