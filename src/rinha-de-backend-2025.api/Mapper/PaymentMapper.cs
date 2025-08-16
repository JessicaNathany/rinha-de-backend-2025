using rinha_de_backend_2025.api.Entity;
using rinha_de_backend_2025.api.Response;

namespace rinha_de_backend_2025.api.Mapper
{
    public class PaymentMapper
    {
        public static PaymentsSummaryResponse ToResponse(List<PaymentSummary> paymentSummary)
        {
            var response = new PaymentsSummaryResponse();
            var resultDefault = paymentSummary.FirstOrDefault(x => x.ServiceUsed == service_used.Default);
            response.@default = ToItem(resultDefault);
            
            var resultFallback = paymentSummary.FirstOrDefault(x => x.ServiceUsed == service_used.Fallback);
            response.fallback = ToItem(resultFallback);
            
            return response;
        }

        private static PaymentsSummaryItem? ToItem(PaymentSummary? paymentSummary)
        {
            if (paymentSummary == null)
                return new PaymentsSummaryItem
                {
                    totalAmount = 0,
                    totalRequests = 0
                };

            return new PaymentsSummaryItem
            {
                totalAmount = paymentSummary.Amount,
                totalRequests = paymentSummary.Request
            };
        }
    }
}
