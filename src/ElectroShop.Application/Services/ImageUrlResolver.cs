using ElectroShop.Application.Common.Options;
using ElectroShop.Domain.Entities;
using Microsoft.Extensions.Options;

namespace ElectroShop.Application.Services;

public class ImageUrlResolver : IImageUrlResolver
{
    private readonly ICloudinaryUrlBuilder _cloudinaryUrlBuilder;
    private readonly ImageStorageOptions _options;

    public ImageUrlResolver(
        ICloudinaryUrlBuilder cloudinaryUrlBuilder,
        IOptions<ImageStorageOptions> options)
    {
        _cloudinaryUrlBuilder = cloudinaryUrlBuilder;
        _options = options.Value;
    }

    public Task<string> ResolveProductImageUrlAsync(
        ProductImage productImage,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(productImage.ImageUrl))
            return Task.FromResult(productImage.ImageUrl);

        if (!string.IsNullOrWhiteSpace(productImage.PublicId))
            return Task.FromResult(_cloudinaryUrlBuilder.BuildSecureUrl(productImage.PublicId));

        if (!string.IsNullOrWhiteSpace(productImage.ImagePath))
        {
            var resolvedPath = ResolvePublicUrl(productImage.ImagePath);
            if (!string.IsNullOrWhiteSpace(resolvedPath))
                return Task.FromResult(resolvedPath);
        }

        return Task.FromResult(_cloudinaryUrlBuilder.BuildSecureUrlFromImageId(productImage.ImageId));
    }

    public string BuildImageUrl(Guid imageId, string? extension = null)
    {
        return _cloudinaryUrlBuilder.BuildSecureUrlFromImageId(imageId);
    }

    public Task<string> BuildImageUrlAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(BuildImageUrl(imageId));
    }

    public string? BuildStaticImageUrl(Guid imageId, string extension)
    {
        return _cloudinaryUrlBuilder.BuildSecureUrlFromImageId(imageId);
    }

    public string? ResolvePublicUrl(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var trimmed = value.Trim();

        if (trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return trimmed;
        }

        if (Guid.TryParse(trimmed, out var imageId))
            return _cloudinaryUrlBuilder.BuildSecureUrlFromImageId(imageId);

        if (trimmed.StartsWith("wwwroot/", StringComparison.OrdinalIgnoreCase) ||
            trimmed.StartsWith("wwwroot\\", StringComparison.OrdinalIgnoreCase))
        {
            trimmed = trimmed["wwwroot".Length..].TrimStart('/', '\\');
        }

        if (trimmed.StartsWith('/'))
            return ToPublicUrl(trimmed);

        if (trimmed.StartsWith("uploads/", StringComparison.OrdinalIgnoreCase) ||
            trimmed.StartsWith("images/", StringComparison.OrdinalIgnoreCase))
        {
            return ToPublicUrl($"/{trimmed}");
        }

        return ToPublicUrl($"/{trimmed}");
    }

    private string ToPublicUrl(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(_options.PublicBaseUrl))
            return relativePath;

        return $"{_options.PublicBaseUrl.TrimEnd('/')}{relativePath}";
    }
}
