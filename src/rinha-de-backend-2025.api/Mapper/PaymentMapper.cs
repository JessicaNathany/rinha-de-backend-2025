using rinha_de_backend_2025.api.Entity;
using rinha_de_backend_2025.api.Response;

namespace rinha_de_backend_2025.api.Mapper
{
    public class PaymentMapper
    {
        public static PaymentsSummaryResponse ToResponse(List<Payments> paymentsSummary, List<Payments> payments)
        {
            var @default = paymentsSummary.FirstOrDefault(x => x.service_used == service_used.Default);
            var fallback = paymentsSummary.FirstOrDefault(x => x.service_used == service_used.Fallback);

            var totalRequestsDefault = payments
                .ToList()
                .Where(x=> x.service_used == service_used.Default).Count();

            var totalRequestsFallback = payments
                .ToList()
                .Where(x => x.service_used == service_used.Fallback).Count();

            return new PaymentsSummaryResponse
            {
                @default = new PaymentsSummaryItem
                {
                    totalRequests = totalRequestsDefault,
                    totalAmount = @default?.amount ?? 0
                },
                fallback = new PaymentsSummaryItem
                {
                    totalRequests = totalRequestsFallback,
                    totalAmount = fallback?.amount ?? 0
                }
            };
        }

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
                return null;
            
            return new PaymentsSummaryItem
            {
                totalAmount = paymentSummary.Amount,
                totalRequests = paymentSummary.Request
            };
        }
    }
}
