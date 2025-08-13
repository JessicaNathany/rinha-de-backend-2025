
namespace rinha_de_backend_2025.api.Response
{
    public class PaymentsSummaryItem
    {
        public int totalRequests { get; set; }
        public decimal totalAmount { get; set; }
    }

    public class PaymentsSummaryResponse
    {
        public PaymentsSummaryItem @default { get; set; }
        public PaymentsSummaryItem fallback { get; set; }
    }
}