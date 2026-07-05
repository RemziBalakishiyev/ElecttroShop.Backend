using ElectroShop.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace ElectroShop.Application.Services;

public class ImageServeService : IImageServeService
{
    private readonly IProductImageQueryRepository _productImageQueryRepository;
    private readonly IImageStorage _imageStorage;
    private readonly ILogger<ImageServeService> _logger;

    public ImageServeService(
        IProductImageQueryRepository productImageQueryRepository,
        IImageStorage imageStorage,
        ILogger<ImageServeService> logger)
    {
        _productImageQueryRepository = productImageQueryRepository;
        _imageStorage = imageStorage;
        _logger = logger;
    }

    public async Task<ImageServeResult?> TryGetImageAsync(
        Guid imageId,
        CancellationToken cancellationToken = default)
    {
        var dbRecord = await _productImageQueryRepository.GetByImageIdAsync(imageId, cancellationToken);

        if (!string.IsNullOrWhiteSpace(dbRecord?.ImageUrl))
        {
            return new ImageServeResult(
                Stream: null,
                ContentType: dbRecord.ContentType,
                RedirectUrl: dbRecord.ImageUrl,
                PhysicalPath: string.Empty,
                DbRecord: dbRecord.ToReference());
        }

        var extension = await _imageStorage.GetImageExtensionAsync(imageId, cancellationToken);
        var physicalPath = _imageStorage.ResolvePhysicalPath(imageId);
        var imageResult = await _imageStorage.GetImageAsync(imageId, cancellationToken);

        if (imageResult != null)
        {
            ProductImageReference? reference = null;
            if (dbRecord != null)
            {
                var fileName = extension != null ? $"{imageId}{extension}" : $"{imageId}.jpg";
                reference = dbRecord.ToReference(fileName);
            }

            return new ImageServeResult(
                imageResult.Value.Stream,
                imageResult.Value.ContentType,
                RedirectUrl: null,
                physicalPath,
                reference);
        }

        if (!string.IsNullOrWhiteSpace(dbRecord?.ImagePath))
        {
            var resolvedPath = dbRecord.ImagePath.StartsWith('/')
                ? dbRecord.ImagePath
                : $"/{dbRecord.ImagePath.TrimStart('/')}";

            return new ImageServeResult(
                Stream: null,
                ContentType: dbRecord.ContentType,
                RedirectUrl: resolvedPath,
                PhysicalPath: physicalPath,
                DbRecord: dbRecord.ToReference());
        }

        var expectedFileName = extension != null ? $"{imageId}{extension}" : $"{imageId}.jpg";
        var dbPath = dbRecord != null
            ? $"ProductImages(ImageId={imageId}, ProductId={dbRecord.ProductId}, ExpectedFile={expectedFileName})"
            : "ProductImage record not found";

        _logger.LogWarning(
            "Image file missing. ImageId: {ImageId}, DbPath: {DbPath}, StorageBasePath: {StorageBasePath}, PhysicalPath: {PhysicalPath}, ImageUrl: {ImageUrl}, PublicId: {PublicId}",
            imageId,
            dbPath,
            _imageStorage.BasePath,
            physicalPath,
            dbRecord?.ImageUrl,
            dbRecord?.PublicId);

        return null;
    }
}

internal static class ProductImageReferenceDtoExtensions
{
    public static ProductImageReference ToReference(this ProductImageReferenceDto dto, string? expectedFileName = null)
    {
        return new ProductImageReference(
            dto.Id,
            dto.ProductId,
            dto.ImageId,
            expectedFileName ?? dto.FileName ?? $"{dto.ImageId}.jpg");
    }
}
