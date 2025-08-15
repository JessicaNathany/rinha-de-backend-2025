using rinha_de_backend_2025.api.Entity;
using rinha_de_backend_2025.api.Response;

namespace rinha_de_backend_2025.api.Infraestrutura
{
    public interface IPaymentRepository
    {
        Task<Payments> Save(Payments entity);
        Task<List<PaymentSummary>> GetPaymentSummary();
        Task<List<Payments>> GetAll();
    }
}
