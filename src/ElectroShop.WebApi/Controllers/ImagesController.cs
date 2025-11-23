using ElectroShop.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroShop.WebApi.Controllers;

/// <summary>
/// Şəkilləri serve etmək üçün controller
/// </summary>
[AllowAnonymous]
[ApiController]
[Route("api/images")]
public class ImagesController : ControllerBase
{
    private readonly IImageStorage _imageStorage;
    private readonly ILogger<ImagesController> _logger;

    public ImagesController(
        IImageStorage imageStorage,
        ILogger<ImagesController> logger)
    {
        _imageStorage = imageStorage;
        _logger = logger;
    }

    /// <summary>
    /// Şəkil əldə edir
    /// </summary>
    /// <param name="imageId">Şəkil ID-si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Şəkil faylı</returns>
    [HttpGet("{imageId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetImage(
        [FromRoute] Guid imageId,
        CancellationToken cancellationToken)
    {
        var imageResult = await _imageStorage.GetImageAsync(imageId, cancellationToken);
        
        if (imageResult == null)
        {
            _logger.LogWarning("Image not found. ImageId: {ImageId}", imageId);
            return NotFound();
        }

        return File(imageResult.Value.Stream, imageResult.Value.ContentType);
    }
}


