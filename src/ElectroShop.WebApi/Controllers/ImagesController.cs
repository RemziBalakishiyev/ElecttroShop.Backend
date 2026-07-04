using ElectroShop.Application.Common.Results;
using ElectroShop.Application.Features.Images.Commands.UploadImage;
using ElectroShop.Application.Services;
using ElectroShop.WebApi.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace ElectroShop.WebApi.Controllers;

/// <summary>
/// Şəkil yükləmə və oxuma üçün controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IImageUploadContext _imageUploadContext;
    private readonly IImageStorage _imageStorage;
    private readonly ILogger<ImagesController> _logger;

    public ImagesController(
        IMediator mediator,
        IImageUploadContext imageUploadContext,
        IImageStorage imageStorage,
        ILogger<ImagesController> logger)
    {
        _mediator = mediator;
        _imageUploadContext = imageUploadContext;
        _imageStorage = imageStorage;
        _logger = logger;
    }

    /// <summary>
    /// Şəkili oxuyur və qaytarır (public endpoint)
    /// Format: GET /api/images/{imageId}.{extension}
    /// </summary>
    [HttpGet("{imageId:guid}.{extension}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetImage(
        [FromRoute] Guid imageId,
        [FromRoute] string extension,
        CancellationToken cancellationToken)
    {
        var imageResult = await _imageStorage.GetImageAsync(imageId, cancellationToken);
        
        if (imageResult == null)
        {
            var searchedPath = _imageStorage.ResolvePhysicalPath(imageId);
            _logger.LogWarning(
                "Image not found: {ImageId}. Searched path: {SearchedPath}. Base path: {BasePath}",
                imageId,
                searchedPath,
                _imageStorage.BasePath);
            return NotFound();
        }

        // Stream-i FileStream kimi qaytar (ASP.NET Core avtomatik dispose edəcək)
        return File(imageResult.Value.Stream, imageResult.Value.ContentType, enableRangeProcessing: true);
    }

    /// <summary>
    /// Şəkili oxuyur və qaytarır (extension olmadan - backward compatibility)
    /// Format: GET /api/images/{imageId}
    /// </summary>
    [HttpGet("{imageId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetImageWithoutExtension(
        [FromRoute] Guid imageId,
        CancellationToken cancellationToken)
    {
        var imageResult = await _imageStorage.GetImageAsync(imageId, cancellationToken);
        
        if (imageResult == null)
        {
            var searchedPath = _imageStorage.ResolvePhysicalPath(imageId);
            _logger.LogWarning(
                "Image not found: {ImageId}. Searched path: {SearchedPath}. Base path: {BasePath}",
                imageId,
                searchedPath,
                _imageStorage.BasePath);
            return NotFound();
        }

        // Stream-i FileStream kimi qaytar (ASP.NET Core avtomatik dispose edəcək)
        return File(imageResult.Value.Stream, imageResult.Value.ContentType, enableRangeProcessing: true);
    }

    /// <summary>
    /// Şəkil yükləyir (product olmadan) və imageId qaytarır
    /// Front-end bu imageId-ləri toplayıb CreateProduct-də istifadə edə bilər
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
        {
            return BadRequest(result);
        }

        return Ok(new { imageId = result.Value });
    }
}
