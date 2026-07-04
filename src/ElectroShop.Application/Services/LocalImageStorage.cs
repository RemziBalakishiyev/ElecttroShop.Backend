using ElectroShop.Application.Common.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ElectroShop.Application.Services;

/// <summary>
/// Lokal storage-da şəkil saxlama implementasiyası (SOLID: Single Responsibility)
/// </summary>
public class LocalImageStorage : IImageStorage
{
    private readonly string _basePath;
    private readonly string? _webRootPath;
    private readonly string? _publicBaseUrl;
    private readonly ILogger<LocalImageStorage> _logger;
    private const int MaxFileSize = 10 * 1024 * 1024; // 10 MB
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
    private static readonly string[] AllowedContentTypes =
    {
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/webp",
        "image/gif"
    };

    public LocalImageStorage(IOptions<ImageStorageOptions> options, ILogger<LocalImageStorage> logger)
    {
        _logger = logger;
        var imageOptions = options.Value;
        _webRootPath = imageOptions.WebRootPath;
        _publicBaseUrl = imageOptions.PublicBaseUrl;
        _basePath = ResolveBasePath(imageOptions);

        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
            _logger.LogInformation("Image storage directory created: {BasePath}", _basePath);
        }
        else
        {
            _logger.LogInformation("Image storage directory resolved: {BasePath}", _basePath);
        }
    }

    public string BasePath => _basePath;

    public string? WebRootPath => _webRootPath;

    public async Task<Guid> UploadImageAsync(
        Stream imageStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        ValidateImage(fileName, contentType, imageStream.Length);

        var imageId = Guid.NewGuid();
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var savedFileName = $"{imageId}{extension}";
        var filePath = Path.Combine(_basePath, savedFileName);

        try
        {
            await using (var fileStream = new FileStream(
                filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,
                useAsync: true))
            {
                await imageStream.CopyToAsync(fileStream, cancellationToken);
                await fileStream.FlushAsync(cancellationToken);
            }

            _logger.LogInformation(
                "Image uploaded successfully. ImageId: {ImageId}, FileName: {FileName}, Path: {Path}, Size: {Size} bytes",
                imageId, savedFileName, filePath, imageStream.Length);

            return imageId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image. ImageId: {ImageId}, Path: {Path}", imageId, filePath);
            throw new InvalidOperationException($"Şəkil yüklənərkən xəta baş verdi: {ex.Message}", ex);
        }
    }

    public async Task DeleteImageAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        var filePath = ResolvePhysicalPath(imageId);

        if (!File.Exists(filePath))
        {
            _logger.LogWarning("Image not found for deletion. ImageId: {ImageId}, Path: {Path}", imageId, filePath);
            return;
        }

        try
        {
            await Task.Run(() => File.Delete(filePath), cancellationToken);
            _logger.LogInformation("Image deleted successfully. ImageId: {ImageId}, Path: {Path}", imageId, filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image. ImageId: {ImageId}, Path: {Path}", imageId, filePath);
            throw new InvalidOperationException($"Şəkil silinərkən xəta baş verdi: {ex.Message}", ex);
        }
    }

    public Task<(Stream Stream, string ContentType)?> GetImageAsync(
        Guid imageId,
        CancellationToken cancellationToken = default)
    {
        var filePath = ResolvePhysicalPath(imageId);

        if (!File.Exists(filePath))
        {
            _logger.LogWarning(
                "Image not found on disk. ImageId: {ImageId}, SearchedPath: {Path}, BasePath: {BasePath}",
                imageId, filePath, _basePath);
            return Task.FromResult<(Stream Stream, string ContentType)?>(null);
        }

        try
        {
            var fileStream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 81920,
                useAsync: true);

            var contentType = GetContentTypeByExtension(Path.GetExtension(filePath));

            return Task.FromResult<(Stream Stream, string ContentType)?>((fileStream, contentType));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading image. ImageId: {ImageId}, Path: {Path}", imageId, filePath);
            return Task.FromResult<(Stream Stream, string ContentType)?>(null);
        }
    }

    public Task<bool> ImageExistsAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        var filePath = ResolvePhysicalPath(imageId);
        return Task.FromResult(File.Exists(filePath));
    }

    public Task<string?> GetImageExtensionAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        foreach (var extension in AllowedExtensions)
        {
            var filePath = Path.Combine(_basePath, $"{imageId}{extension}");
            if (File.Exists(filePath))
                return Task.FromResult<string?>(extension);
        }

        return Task.FromResult<string?>(null);
    }

    public string ResolvePhysicalPath(Guid imageId)
    {
        foreach (var extension in AllowedExtensions)
        {
            var filePath = Path.Combine(_basePath, $"{imageId}{extension}");
            if (File.Exists(filePath))
                return filePath;
        }

        return Path.Combine(_basePath, $"{imageId}.jpg");
    }

    public IReadOnlyList<StoredImageFileInfo> ListStoredImages(int maxCount = 50)
    {
        if (!Directory.Exists(_basePath))
            return [];

        return Directory.EnumerateFiles(_basePath)
            .OrderByDescending(File.GetLastWriteTimeUtc)
            .Take(maxCount)
            .Select(filePath =>
            {
                var fileName = Path.GetFileName(filePath);
                var relativePath = BuildRelativePublicPath(fileName);
                return new StoredImageFileInfo(
                    fileName,
                    relativePath,
                    BuildPublicUrl(relativePath),
                    new FileInfo(filePath).Length);
            })
            .ToList();
    }

    private static string ResolveBasePath(ImageStorageOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.BasePath) && Path.IsPathRooted(options.BasePath))
            return Path.GetFullPath(options.BasePath);

        var configured = string.IsNullOrWhiteSpace(options.BasePath)
            ? Path.Combine("wwwroot", "images", "products")
            : options.BasePath.Replace('\\', '/');

        var contentRoot = !string.IsNullOrWhiteSpace(options.ContentRootPath)
            ? options.ContentRootPath
            : Directory.GetCurrentDirectory();

        if (configured.StartsWith("wwwroot/", StringComparison.OrdinalIgnoreCase))
            configured = configured["wwwroot/".Length..];

        if (configured.StartsWith('/'))
            configured = configured.TrimStart('/');

        var webRoot = !string.IsNullOrWhiteSpace(options.WebRootPath)
            ? options.WebRootPath
            : Path.Combine(contentRoot, "wwwroot");

        if (configured.StartsWith("uploads/", StringComparison.OrdinalIgnoreCase))
            return Path.GetFullPath(Path.Combine(webRoot, configured));

        if (configured.StartsWith("images/", StringComparison.OrdinalIgnoreCase))
            return Path.GetFullPath(Path.Combine(webRoot, configured));

        return Path.GetFullPath(Path.Combine(contentRoot, configured));
    }

    private string BuildRelativePublicPath(string fileName)
    {
        if (_webRootPath != null &&
            _basePath.StartsWith(_webRootPath, StringComparison.OrdinalIgnoreCase))
        {
            var relative = Path.GetRelativePath(_webRootPath, Path.Combine(_basePath, fileName))
                .Replace('\\', '/');
            return $"/{relative}";
        }

        return $"/images/products/{fileName}";
    }

    private string BuildPublicUrl(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(_publicBaseUrl))
            return relativePath;

        return $"{_publicBaseUrl.TrimEnd('/')}{relativePath}";
    }

    private static void ValidateImage(string fileName, string contentType, long fileSize)
    {
        if (fileSize > MaxFileSize)
            throw new ArgumentException($"Şəkil ölçüsü maksimum {MaxFileSize / (1024 * 1024)} MB ola bilər");

        if (fileSize == 0)
            throw new ArgumentException("Şəkil boş ola bilməz");

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
            throw new ArgumentException($"İcazə verilən fayl formatları: {string.Join(", ", AllowedExtensions)}");

        if (!AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException($"İcazə verilən content type-lar: {string.Join(", ", AllowedContentTypes)}");
    }

    private static string GetContentTypeByExtension(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            ".gif" => "image/gif",
            _ => "image/jpeg"
        };
    }
}
