namespace rinha_de_backend_2025.api.Entity
{
    public class Payments
    {
        public int id { get; set; }
        public Guid correlation_id { get; set; }
        public decimal amount { get; set; }
        public DateTime requested_at { get; set; }
        public service_used service_used { get; set; }
    }
}
