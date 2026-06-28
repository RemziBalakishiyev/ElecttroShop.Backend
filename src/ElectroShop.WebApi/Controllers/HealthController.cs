using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroShop.WebApi.Controllers;

/// <summary>
/// Health check üçün Controller
/// </summary>
[ApiController]
[Route("api/health")]
[AllowAnonymous]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Servisin sağlamlığını yoxlayır
    /// </summary>
    /// <returns>Servis statusu</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetHealth()
    {
        return Ok(new { status = "ok" });
    }
}
