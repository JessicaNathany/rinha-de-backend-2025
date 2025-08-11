namespace rinha_de_backend_2025.api.Entity
{
    public class Payments
    {
        public int Id { get; set; }
        public Guid CorrelationId { get; set; }
        public decimal Amount { get; set; }
        public DateTime RequestedAt { get; set; }
        public ServiceType ServiceType { get; set; }
    }
}
