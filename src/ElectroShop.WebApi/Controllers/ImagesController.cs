using ElectroShop.Application.Common.Results;
using ElectroShop.Application.Features.Images.Commands.UploadImage;
using ElectroShop.Application.Services;
using ElectroShop.WebApi.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroShop.WebApi.Controllers;

/// <summary>
/// Şəkil yükləmə və oxuma üçün controller.
/// [ApiController] istifadə edilmir — img src üçün 404-də JSON/problem+json yaranmasın (ORB).
/// </summary>
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IImageUploadContext _imageUploadContext;
    private readonly IImageServeService _imageServeService;
    private readonly ILogger<ImagesController> _logger;

    public ImagesController(
        IMediator mediator,
        IImageUploadContext imageUploadContext,
        IImageServeService imageServeService,
        ILogger<ImagesController> logger)
    {
        _mediator = mediator;
        _imageUploadContext = imageUploadContext;
        _imageServeService = imageServeService;
        _logger = logger;
    }

    /// <summary>
    /// Şəkili oxuyur və qaytarır (public endpoint)
    /// Format: GET /api/images/{imageId}.{extension}
    /// </summary>
    [HttpGet("{imageId:guid}.{extension}")]
    [AllowAnonymous]
    [Produces("image/jpeg", "image/png", "image/webp", "image/gif")]
    public Task<IActionResult> GetImage(
        [FromRoute] Guid imageId,
        [FromRoute] string extension,
        CancellationToken cancellationToken)
        => ServeImageAsync(imageId, cancellationToken);

    /// <summary>
    /// Şəkili oxuyur və qaytarır (extension olmadan)
    /// Format: GET /api/images/{imageId}
    /// </summary>
    [HttpGet("{imageId:guid}")]
    [AllowAnonymous]
    [Produces("image/jpeg", "image/png", "image/webp", "image/gif")]
    public Task<IActionResult> GetImageWithoutExtension(
        [FromRoute] Guid imageId,
        CancellationToken cancellationToken)
        => ServeImageAsync(imageId, cancellationToken);

    /// <summary>
    /// Şəkil yükləyir (product olmadan) və imageId qaytarır
    /// </summary>
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadImage(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is empty");

        var commandResult = await FileUploadHelper
            .CreateUploadImageCommandAsync(file, _imageUploadContext, cancellationToken);

        if (commandResult.IsFailure)
        {
            _logger.LogWarning(
                "Image upload validation failed. Error: {Error}",
                commandResult.Error.Message);

            return BadRequest(Result.Failure(commandResult.Error));
        }

        var result = await _mediator.Send(commandResult.Value, cancellationToken);

        if (result.IsFailure)
            return BadRequest(result);

        return Ok(new { imageId = result.Value });
    }

    private async Task<IActionResult> ServeImageAsync(Guid imageId, CancellationToken cancellationToken)
    {
        var image = await _imageServeService.TryGetImageAsync(imageId, cancellationToken);

        if (image == null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            Response.ContentType = "image/jpeg";
            return new EmptyResult();
        }

        Response.Headers.CacheControl = "public,max-age=86400";
        return File(image.Stream, image.ContentType, enableRangeProcessing: true);
    }
}
