using Microsoft.AspNetCore.Mvc;
using rinha_de_backend_2025.api.Request;
using rinha_de_backend_2025.api.Service;

namespace rinha_de_backend_2025.api.Controllers
{
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentProcessor _paymentProcessor;

        public PaymentController(IPaymentProcessor paymentProcessor)
        {
            _paymentProcessor = paymentProcessor;
        }

        [HttpPost]
        [Route("payment")]
        public async Task<IActionResult> Payment(PaymentRequest request)
        {
            try
            {
                //await _paymentProcessor.PaymentProcessorDefault(request);
                // return Created();

                return Ok(new
                {
                    correlationId = "4a7901b8-7d26-4d9d-aa19-4dc1c7cf60b5",
                    amount = 59.90,
                    requestedAt = "2025-07-15T12:34:56.000Z"
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("payment-summary")]
        public async Task<IActionResult> PaymentSummary()
        {
            return Ok(new
            {
                @default = new { totalRequests = 10, totalAmount = 1000.00m },
                fallback = new { totalRequests = 2, totalAmount = 200.00m }
            });
        }
    }
}
