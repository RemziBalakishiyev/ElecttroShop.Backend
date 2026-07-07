using ElectroShop.Application.Common.Results;
using ElectroShop.Application.Features.Images.Commands.UploadImage;
using ElectroShop.Application.Services;
using ElectroShop.WebApi.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroShop.WebApi.Controllers;

/// <summary>
/// Şəkil yükləmə üçün controller. Oxuma Cloudinary URL-ləri vasitəsilə həyata keçirilir.
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
    /// Köhnə /api/images/{imageId} sorğularını Cloudinary-ə yönləndirir (geri uyğunluq).
    /// </summary>
    [HttpGet("{imageId:guid}.{extension}")]
    [HttpGet("{imageId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> RedirectToCloudinary(
        [FromRoute] Guid imageId,
        CancellationToken cancellationToken)
    {
        var image = await _imageServeService.TryGetImageAsync(imageId, cancellationToken);

        if (image?.RedirectUrl == null)
        {
            _logger.LogWarning("Cloudinary redirect failed. ImageId: {ImageId}", imageId);
            return NotFound();
        }

        return RedirectPermanent(image.RedirectUrl);
    }

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
}
