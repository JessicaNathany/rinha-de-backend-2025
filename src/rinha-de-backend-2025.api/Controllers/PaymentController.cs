using Microsoft.AspNetCore.Mvc;
using rinha_de_backend_2025.api.Mapper;
using rinha_de_backend_2025.api.Infraestrutura.Queues;
using rinha_de_backend_2025.api.Request;
using rinha_de_backend_2025.api.Service;

namespace rinha_de_backend_2025.api.Controllers;

[ApiController]
public class PaymentController(IPaymentMessageQueue paymentMessageQueue, IPaymentProcessor paymentProcessor, IPaymentManager paymentManager) : ControllerBase
{
    [HttpPost]
    [Route("payment")]
    public async Task<IActionResult> Payment(PaymentRequest request, CancellationToken cancellationToken)
    {
        await paymentMessageQueue.PublishAsync(request, cancellationToken);
        return Accepted();
    }

    [HttpGet]
    [Route("payment-summary")]
    public async Task<IActionResult> PaymentSummary()
    {
        var paymentSummary = await paymentProcessor.GetPaymentSummary();
        var result = PaymentMapper.ToResponse(paymentSummary);

        return Ok(result);
    }
    
    // For tests 
    [HttpPost]
    [Route("payment/sync")]
    public async Task<IActionResult> PaymentSync(PaymentRequest request, CancellationToken cancellationToken)
    {
        await paymentManager.SubmitPayment(request, cancellationToken);
        return Accepted();
    }
}