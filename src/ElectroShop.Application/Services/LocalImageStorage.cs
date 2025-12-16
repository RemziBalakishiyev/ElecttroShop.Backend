using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;

namespace ElectroShop.Application.Services;

/// <summary>
/// Lokal storage-da şəkil saxlama implementasiyası (SOLID: Single Responsibility)
/// </summary>
public class LocalImageStorage : IImageStorage
{
    private readonly string _basePath;
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

    public LocalImageStorage(IConfiguration configuration, ILogger<LocalImageStorage> logger)
    {
        _logger = logger;
        _basePath = configuration["ImageStorage:BasePath"] 
            ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");

        // Əgər qovluq yoxdursa yarat
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
            _logger.LogInformation("Image storage directory created: {BasePath}", _basePath);
        }
    }

    public async Task<Guid> UploadImageAsync(
        Stream imageStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        // Validasiya
        ValidateImage(fileName, contentType, imageStream.Length);

        // Unikal ID yarat
        var imageId = Guid.NewGuid();
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var savedFileName = $"{imageId}{extension}";
        var filePath = Path.Combine(_basePath, savedFileName);

        try
        {
            // Stream-based async yükləmə (performans üçün)
            await using (var fileStream = new FileStream(
                filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920, // 80 KB buffer (optimize edilmiş)
                useAsync: true))
            {
                await imageStream.CopyToAsync(fileStream, cancellationToken);
                await fileStream.FlushAsync(cancellationToken);
            }

            _logger.LogInformation(
                "Image uploaded successfully. ImageId: {ImageId}, FileName: {FileName}, Size: {Size} bytes",
                imageId, savedFileName, imageStream.Length);

            return imageId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image. ImageId: {ImageId}", imageId);
            throw new InvalidOperationException($"Şəkil yüklənərkən xəta baş verdi: {ex.Message}", ex);
        }
    }

    public async Task DeleteImageAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        var filePath = GetImagePath(imageId);

        if (!File.Exists(filePath))
        {
            _logger.LogWarning("Image not found for deletion. ImageId: {ImageId}", imageId);
            return;
        }

        try
        {
            await Task.Run(() => File.Delete(filePath), cancellationToken);
            _logger.LogInformation("Image deleted successfully. ImageId: {ImageId}", imageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image. ImageId: {ImageId}", imageId);
            throw new InvalidOperationException($"Şəkil silinərkən xəta baş verdi: {ex.Message}", ex);
        }
    }

    public Task<(Stream Stream, string ContentType)?> GetImageAsync(
        Guid imageId,
        CancellationToken cancellationToken = default)
    {
        var filePath = GetImagePath(imageId);

        if (!File.Exists(filePath))
        {
            _logger.LogWarning("Image not found. ImageId: {ImageId}", imageId);
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
            _logger.LogError(ex, "Error reading image. ImageId: {ImageId}", imageId);
            return Task.FromResult<(Stream Stream, string ContentType)?>(null);
        }
    }

    public Task<bool> ImageExistsAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        var filePath = GetImagePath(imageId);
        var exists = File.Exists(filePath);
        return Task.FromResult(exists);
    }

    public Task<string?> GetImageExtensionAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        // Bütün mümkün uzantıları yoxla
        foreach (var extension in AllowedExtensions)
        {
            var filePath = Path.Combine(_basePath, $"{imageId}{extension}");
            if (File.Exists(filePath))
                return Task.FromResult<string?>(extension);
        }

        // Əgər heç biri tapılmadısa, null qaytar
        return Task.FromResult<string?>(null);
    }

    private string GetImagePath(Guid imageId)
    {
        // Bütün mümkün uzantıları yoxla
        foreach (var extension in AllowedExtensions)
        {
            var filePath = Path.Combine(_basePath, $"{imageId}{extension}");
            if (File.Exists(filePath))
                return filePath;
        }

        // Əgər heç biri tapılmadısa, default olaraq .jpg istifadə et
        return Path.Combine(_basePath, $"{imageId}.jpg");
    }

    private static void ValidateImage(string fileName, string contentType, long fileSize)
    {
        // Fayl ölçüsü yoxlaması
        if (fileSize > MaxFileSize)
        {
            throw new ArgumentException($"Şəkil ölçüsü maksimum {MaxFileSize / (1024 * 1024)} MB ola bilər");
        }

        if (fileSize == 0)
        {
            throw new ArgumentException("Şəkil boş ola bilməz");
        }

        // Uzantı yoxlaması
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
        {
            throw new ArgumentException($"İcazə verilən fayl formatları: {string.Join(", ", AllowedExtensions)}");
        }

        // Content type yoxlaması
        if (!AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException($"İcazə verilən content type-lar: {string.Join(", ", AllowedContentTypes)}");
        }
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

