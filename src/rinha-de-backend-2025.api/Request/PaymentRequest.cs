namespace rinha_de_backend_2025.api.Request
{
    public class PaymentRequest
    {
        public string CorrelationId { get; set; }
        public decimal Amount { get; set; }
    }
}
