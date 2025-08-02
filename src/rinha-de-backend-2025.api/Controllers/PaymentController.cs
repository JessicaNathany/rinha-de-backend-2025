using Microsoft.AspNetCore.Mvc;
using rinha_de_backend_2025.api.Request;

namespace rinha_de_backend_2025.api.Controllers
{
    [ApiController]
    public class PaymentController : ControllerBase
    {
        [HttpPost]
        [Route("payment")]
        public async Task<IActionResult> Payment(PaymentRequest request)
        {
            return Created();

        }

        [HttpGet]
        [Route("payment-summary")]
        public async Task<IActionResult> PaymentSummary()
        {
            return Ok();

        }
    }
}
