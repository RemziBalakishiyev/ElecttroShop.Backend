using ElectroShop.Application.DTOs;
using ElectroShop.Application.Features.Images.Commands.BackfillCloudinaryImages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroShop.WebApi.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin")]
public class AdminController : BaseApiController
{
    [HttpPost("backfill-cloudinary-images")]
    [ProducesResponseType(typeof(BackfillCloudinaryImagesResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> BackfillCloudinaryImages(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new BackfillCloudinaryImagesCommand(), cancellationToken);
        return HandleResult(result);
    }
}
