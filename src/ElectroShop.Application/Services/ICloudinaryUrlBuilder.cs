namespace ElectroShop.Application.Services;

public interface ICloudinaryUrlBuilder
{
    string BuildSecureUrl(string publicId);

    string BuildSecureUrlFromImageId(Guid imageId);

    string BuildPublicIdFromImageId(Guid imageId);
}
