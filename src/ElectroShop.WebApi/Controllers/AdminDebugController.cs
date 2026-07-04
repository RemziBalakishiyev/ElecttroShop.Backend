using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using ElectroShop.Domain.Entities;
using ElectroShop.Persistence.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectroShop.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/admin/debug")]
public class AdminDebugController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly IImageStorage _imageStorage;
    private readonly IImageUrlResolver _imageUrlResolver;
    private readonly ElectroShopDbContext _dbContext;

    public AdminDebugController(
        IWebHostEnvironment environment,
        IImageStorage imageStorage,
        IImageUrlResolver imageUrlResolver,
        ElectroShopDbContext dbContext)
    {
        _environment = environment;
        _imageStorage = imageStorage;
        _imageUrlResolver = imageUrlResolver;
        _dbContext = dbContext;
    }

    [HttpGet("uploads")]
    [ProducesResponseType(typeof(UploadsDebugResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<UploadsDebugResponse> GetUploadsDebug()
    {
        var webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        var files = _imageStorage.ListStoredImages(50);

        return Ok(new UploadsDebugResponse
        {
            WebRootPath = webRootPath,
            ContentRootPath = _environment.ContentRootPath,
            ImageStorageBasePath = _imageStorage.BasePath,
            UploadsFolderExists = Directory.Exists(_imageStorage.BasePath),
            FileCount = Directory.Exists(_imageStorage.BasePath)
                ? Directory.EnumerateFiles(_imageStorage.BasePath).Count()
                : 0,
            Files = files.Select(file => new UploadDebugFileDto
            {
                FileName = file.FileName,
                RelativePath = file.RelativePath,
                PublicUrl = file.PublicUrl,
                Size = file.Size
            }).ToList()
        });
    }

    [HttpGet("image/{id:guid}")]
    [ProducesResponseType(typeof(ImageDebugResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ImageDebugResponse>> GetImageDebug(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var productImage = await _dbContext.Set<ProductImage>()
            .AsNoTracking()
            .FirstOrDefaultAsync(pi => pi.ImageId == id, cancellationToken);

        var physicalPath = _imageStorage.ResolvePhysicalPath(id);
        var fileExists = System.IO.File.Exists(physicalPath);
        var extension = await _imageStorage.GetImageExtensionAsync(id, cancellationToken);

        return Ok(new ImageDebugResponse
        {
            ImageId = id,
            ImageRecordFound = productImage != null,
            ProductImageId = productImage?.Id,
            ProductId = productImage?.ProductId,
            StoredPath = _imageStorage.BasePath,
            StoredFileName = extension != null ? $"{id}{extension}" : $"{id}.jpg",
            PhysicalPathSearched = physicalPath,
            FileExists = fileExists,
            DetectedExtension = extension,
            PublicUrl = await _imageUrlResolver.BuildImageUrlAsync(id, cancellationToken),
            StaticPublicUrl = extension != null
                ? _imageUrlResolver.BuildStaticImageUrl(id, extension)
                : null
        });
    }
}
