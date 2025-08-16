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
    [Route("payments")]
    public async Task<IActionResult> Payment(PaymentRequest request, CancellationToken cancellationToken)
    {
        await paymentMessageQueue.PublishAsync(request, cancellationToken);
        return Accepted();
    }

    [HttpGet]
    [Route("payments-summary")]
    public async Task<IActionResult> PaymentSummary([FromQuery]DateTime? to, [FromQuery] DateTime? from)
    {
        if (to.HasValue && from.HasValue && from > to)
            return BadRequest("Start date cannot be greater than end date.");

        var paymentSummary  = await paymentProcessor.GetSummaryAndAll(to, from);
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