using rinha_de_backend_2025.api.Entity;
using rinha_de_backend_2025.api.Infraestrutura;
using rinha_de_backend_2025.api.Request;
using rinha_de_backend_2025.api.Infraestrutura.Clients;

namespace rinha_de_backend_2025.api.Service;

public class PaymentProcessor(IPaymentGatewayClient paymentGatewayClient, IPaymentRepository paymentRepository) : IPaymentProcessor
{
    public async Task<Payments> PaymentProcessorDefault(PaymentRequest request)
    {
        var payments = await paymentGatewayClient.ProcessPaymentDefault(request);
        
        return await paymentRepository.Save(payments);
    }

    public async Task<Payments> PaymentProcessorFallback(PaymentRequest request)
    {
       var payments = await paymentGatewayClient.ProcessPaymentFallback(request);

       return await paymentRepository.Save(payments);
    }

    public async Task<List<PaymentSummary>> GetSummaryAndAll(DateTime? startDate, DateTime? endDate)
    {
        return await paymentRepository.GetPaymentSummary(startDate, endDate);
    }
}