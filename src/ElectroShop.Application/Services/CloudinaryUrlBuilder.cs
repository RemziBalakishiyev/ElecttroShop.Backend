using ElectroShop.Application.Common.Options;
using Microsoft.Extensions.Options;

namespace ElectroShop.Application.Services;

public class CloudinaryUrlBuilder : ICloudinaryUrlBuilder
{
    private readonly CloudinarySettings _settings;

    public CloudinaryUrlBuilder(IOptions<CloudinarySettings> settings)
    {
        _settings = settings.Value;
    }

    public string BuildSecureUrl(string publicId)
    {
        var cloudName = _settings.CloudName.Trim();
        var normalizedPublicId = publicId.Trim().TrimStart('/');
        return $"https://res.cloudinary.com/{cloudName}/image/upload/{normalizedPublicId}";
    }

    public string BuildSecureUrlFromImageId(Guid imageId)
    {
        return BuildSecureUrl(BuildPublicIdFromImageId(imageId));
    }

    public string BuildPublicIdFromImageId(Guid imageId)
    {
        var folder = _settings.Folder.Trim().Trim('/');
        return $"{folder}/{imageId}";
    }
}
