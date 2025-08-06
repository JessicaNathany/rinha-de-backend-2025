using Microsoft.AspNetCore.Mvc;

namespace rinha_de_backend_2025.api.Controllers;

[ApiController]
[Route("api/temp")]
public class TempController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var message = new Message("Hello Service");
        return Ok(message);
    }

    record Message(string message);
}