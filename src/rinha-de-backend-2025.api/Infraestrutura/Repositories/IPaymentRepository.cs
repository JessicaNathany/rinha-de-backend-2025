using rinha_de_backend_2025.api.Entity;

namespace rinha_de_backend_2025.api.Infraestrutura.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payments> Save(Payments entity);
        Task<List<PaymentSummary>> GetPaymentSummary(DateTime? startDate, DateTime? endDate);
        Task<List<Payments>> GetAll();
    }
}
