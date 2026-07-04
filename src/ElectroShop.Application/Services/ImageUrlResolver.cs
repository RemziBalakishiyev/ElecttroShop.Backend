using ElectroShop.Application.Common.Options;
using Microsoft.Extensions.Options;

namespace ElectroShop.Application.Services;

public class ImageUrlResolver : IImageUrlResolver
{
    private readonly IImageStorage _imageStorage;
    private readonly ImageStorageOptions _options;

    public ImageUrlResolver(IImageStorage imageStorage, IOptions<ImageStorageOptions> options)
    {
        _imageStorage = imageStorage;
        _options = options.Value;
    }

    public string BuildImageUrl(Guid imageId, string? extension = null)
    {
        var normalizedExtension = NormalizeExtension(extension);
        if (normalizedExtension != null)
        {
            var staticUrl = BuildStaticImageUrl(imageId, normalizedExtension);
            if (staticUrl != null)
                return staticUrl;
        }

        var relativePath = normalizedExtension != null
            ? $"/api/images/{imageId}{normalizedExtension}"
            : $"/api/images/{imageId}";

        return ToPublicUrl(relativePath);
    }

    public async Task<string> BuildImageUrlAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        var extension = await _imageStorage.GetImageExtensionAsync(imageId, cancellationToken);
        return BuildImageUrl(imageId, extension);
    }

    public string? BuildStaticImageUrl(Guid imageId, string extension)
    {
        var normalizedExtension = NormalizeExtension(extension);
        if (normalizedExtension == null)
            return null;

        var relativePath = $"/images/products/{imageId}{normalizedExtension}";
        return ToPublicUrl(relativePath);
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
            return BuildImageUrl(imageId);

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

    private static string? NormalizeExtension(string? extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
            return null;

        return extension.StartsWith('.') ? extension.ToLowerInvariant() : $".{extension.ToLowerInvariant()}";
    }
}
