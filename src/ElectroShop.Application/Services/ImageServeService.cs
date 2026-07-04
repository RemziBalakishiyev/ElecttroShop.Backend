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
        var extension = await _imageStorage.GetImageExtensionAsync(imageId, cancellationToken);
        var physicalPath = _imageStorage.ResolvePhysicalPath(imageId);
        var imageResult = await _imageStorage.GetImageAsync(imageId, cancellationToken);

        if (imageResult != null)
        {
            ProductImageReference? reference = null;
            if (dbRecord != null)
            {
                var fileName = extension != null ? $"{imageId}{extension}" : $"{imageId}.jpg";
                reference = new ProductImageReference(
                    dbRecord.Id,
                    dbRecord.ProductId,
                    dbRecord.ImageId,
                    fileName);
            }

            return new ImageServeResult(
                imageResult.Value.Stream,
                imageResult.Value.ContentType,
                physicalPath,
                reference);
        }

        var expectedFileName = extension != null ? $"{imageId}{extension}" : $"{imageId}.jpg";
        var dbPath = dbRecord != null
            ? $"ProductImages(ImageId={imageId}, ProductId={dbRecord.ProductId}, ExpectedFile={expectedFileName})"
            : "ProductImage record not found";

        _logger.LogWarning(
            "Image file missing. ImageId: {ImageId}, DbPath: {DbPath}, StorageBasePath: {StorageBasePath}, PhysicalPath: {PhysicalPath}",
            imageId,
            dbPath,
            _imageStorage.BasePath,
            physicalPath);

        return null;
    }
}
