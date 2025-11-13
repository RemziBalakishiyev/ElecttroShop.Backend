using ElectroShop.Application.Features.Auth.Commands.Login;
using ElectroShop.Application.Features.Auth.Commands.RefreshToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroShop.WebApi.Controllers;

[AllowAnonymous]
public class AuthController : BaseApiController
{
    /// <summary>
    /// İstifadəçi girişi
    /// </summary>
    /// <param name="command">Login məlumatları (Email və Password)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Access token və Refresh token</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Access token yeniləmək üçün Refresh token istifadə edir
    /// </summary>
    /// <param name="command">Refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Yeni Access token və Refresh token</returns>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}

