using rinha_de_backend_2025.api.Entity;
using rinha_de_backend_2025.api.Response;

namespace rinha_de_backend_2025.api.Mapper
{
    public class PaymentMapper
    {
        public static PaymentsSummaryResponse ToResponse(List<Payments> payments)
        {
            var @default = payments.FirstOrDefault(x => x.service_used == service_used.Default);
            var fallback = payments.FirstOrDefault(x => x.service_used == service_used.Fallback);

            return new PaymentsSummaryResponse
            {
                @default = new PaymentsSummaryItem
                {
                    totalRequests = @default != null ? (int)@default.id : 0, 
                    totalAmount = @default?.amount ?? 0
                },
                fallback = new PaymentsSummaryItem
                {
                    totalRequests = fallback != null ? (int)fallback.id : 0, 
                    totalAmount = fallback?.amount ?? 0
                }
            };
        }
    }
}
