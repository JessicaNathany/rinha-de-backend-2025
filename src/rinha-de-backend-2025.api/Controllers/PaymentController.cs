using Microsoft.AspNetCore.Mvc;
using rinha_de_backend_2025.api.Entity;
using rinha_de_backend_2025.api.Mapper;
using rinha_de_backend_2025.api.Infraestrutura.Queues;
using rinha_de_backend_2025.api.Request;
using rinha_de_backend_2025.api.Service;

namespace rinha_de_backend_2025.api.Controllers
{
    [ApiController]
    public class PaymentController(IPaymentMessageQueue paymentMessageQueue, IPaymentProcessor paymentProcessor) : ControllerBase
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
            try
            {
                var paymentSummary = await _paymentProcessor.GetPaymentSummary();
                var result = PaymentMapper.ToResponse(paymentSummary);

                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
