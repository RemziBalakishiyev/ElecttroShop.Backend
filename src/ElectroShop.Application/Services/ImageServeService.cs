using ElectroShop.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace ElectroShop.Application.Services;

public class ImageServeService : IImageServeService
{
    private readonly IProductImageQueryRepository _productImageQueryRepository;
    private readonly ICloudinaryUrlBuilder _cloudinaryUrlBuilder;
    private readonly ILogger<ImageServeService> _logger;

    public ImageServeService(
        IProductImageQueryRepository productImageQueryRepository,
        ICloudinaryUrlBuilder cloudinaryUrlBuilder,
        ILogger<ImageServeService> logger)
    {
        _productImageQueryRepository = productImageQueryRepository;
        _cloudinaryUrlBuilder = cloudinaryUrlBuilder;
        _logger = logger;
    }

    public async Task<ImageServeResult?> TryGetImageAsync(
        Guid imageId,
        CancellationToken cancellationToken = default)
    {
        var dbRecord = await _productImageQueryRepository.GetByImageIdAsync(imageId, cancellationToken);

        string? redirectUrl = null;

        if (!string.IsNullOrWhiteSpace(dbRecord?.ImageUrl))
        {
            redirectUrl = dbRecord.ImageUrl;
        }
        else if (!string.IsNullOrWhiteSpace(dbRecord?.PublicId))
        {
            redirectUrl = _cloudinaryUrlBuilder.BuildSecureUrl(dbRecord.PublicId);
        }
        else
        {
            redirectUrl = _cloudinaryUrlBuilder.BuildSecureUrlFromImageId(imageId);
        }

        _logger.LogInformation(
            "Redirecting legacy image request to Cloudinary. ImageId: {ImageId}, RedirectUrl: {RedirectUrl}",
            imageId,
            redirectUrl);

        return new ImageServeResult(
            Stream: null,
            ContentType: dbRecord?.ContentType,
            RedirectUrl: redirectUrl,
            PhysicalPath: string.Empty,
            DbRecord: dbRecord?.ToReference());
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
