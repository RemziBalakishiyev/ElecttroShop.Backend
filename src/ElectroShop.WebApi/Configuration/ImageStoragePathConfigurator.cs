using ElectroShop.Application.Common.Options;
using Microsoft.AspNetCore.Hosting;

namespace ElectroShop.WebApi.Configuration;

public static class ImageStoragePathConfigurator
{
    public static void ConfigureImageStorage(this WebApplicationBuilder builder)
    {
        var contentRoot = builder.Environment.ContentRootPath;
        var webRoot = builder.Environment.WebRootPath ?? Path.Combine(contentRoot, "wwwroot");

        if (!Directory.Exists(webRoot))
            Directory.CreateDirectory(webRoot);

        var configuredBasePath = builder.Configuration[$"{ImageStorageOptions.SectionName}:BasePath"]
            ?? Path.Combine("wwwroot", "images", "products");

        var absoluteBasePath = Path.IsPathRooted(configuredBasePath)
            ? Path.GetFullPath(configuredBasePath)
            : ResolveRelativeStoragePath(contentRoot, webRoot, configuredBasePath);

        if (!Directory.Exists(absoluteBasePath))
            Directory.CreateDirectory(absoluteBasePath);

        builder.Services.Configure<ImageStorageOptions>(options =>
        {
            builder.Configuration.GetSection(ImageStorageOptions.SectionName).Bind(options);
            options.BasePath = absoluteBasePath;
            options.ContentRootPath = contentRoot;
            options.WebRootPath = webRoot;
            options.PublicBaseUrl = builder.Configuration["PUBLIC_BASE_URL"];
        });
    }

    private static string ResolveRelativeStoragePath(string contentRoot, string webRoot, string configuredBasePath)
    {
        var normalized = configuredBasePath.Replace('\\', '/');

        if (normalized.StartsWith("wwwroot/", StringComparison.OrdinalIgnoreCase))
            normalized = normalized["wwwroot/".Length..];

        normalized = normalized.TrimStart('/');

        if (normalized.StartsWith("uploads/", StringComparison.OrdinalIgnoreCase) ||
            normalized.StartsWith("images/", StringComparison.OrdinalIgnoreCase))
        {
            return Path.GetFullPath(Path.Combine(webRoot, normalized));
        }

        return Path.GetFullPath(Path.Combine(contentRoot, configuredBasePath));
    }
}
