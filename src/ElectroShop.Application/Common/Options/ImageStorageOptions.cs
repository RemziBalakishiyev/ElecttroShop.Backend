namespace ElectroShop.Application.Common.Options;

public class ImageStorageOptions
{
    public const string SectionName = "ImageStorage";

    /// <summary>
    /// Absolute path to the directory where product images are stored.
    /// </summary>
    public string BasePath { get; set; } = string.Empty;

    /// <summary>
    /// ASP.NET Core content root path (set at startup from WebApi layer).
    /// </summary>
    public string? ContentRootPath { get; set; }

    /// <summary>
    /// ASP.NET Core web root path (typically {ContentRoot}/wwwroot).
    /// </summary>
    public string? WebRootPath { get; set; }

    /// <summary>
    /// Public base URL for absolute image links in API responses (e.g. https://api.smartal.net).
    /// </summary>
    public string? PublicBaseUrl { get; set; }
}
