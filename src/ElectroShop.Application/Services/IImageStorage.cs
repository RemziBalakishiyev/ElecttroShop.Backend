namespace ElectroShop.Application.Services;

public record StoredImageFileInfo(
    string FileName,
    string RelativePath,
    string PublicUrl,
    long Size);

/// <summary>
/// Şəkil saxlama servisi interfeysi (SOLID: Interface Segregation)
/// </summary>
public interface IImageStorage
{
    string BasePath { get; }

    string? WebRootPath { get; }

    /// <summary>
    /// Şəkil yükləyir və unikal ID qaytarır
    /// </summary>
    Task<Guid> UploadImageAsync(
        Stream imageStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Şəkili silir
    /// </summary>
    Task DeleteImageAsync(Guid imageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Şəkili oxuyur
    /// </summary>
    Task<(Stream Stream, string ContentType)?> GetImageAsync(
        Guid imageId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Şəklin mövcud olub-olmadığını yoxlayır
    /// </summary>
    Task<bool> ImageExistsAsync(Guid imageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Şəklin extension-ını qaytarır
    /// </summary>
    Task<string?> GetImageExtensionAsync(Guid imageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Diskdə axtarılan fiziki path-i qaytarır (fayl tapılmasa da son yoxlanan path).
    /// </summary>
    string ResolvePhysicalPath(Guid imageId);

    /// <summary>
    /// Storage qovluğundakı faylları siyahıya alır (debug üçün).
    /// </summary>
    IReadOnlyList<StoredImageFileInfo> ListStoredImages(int maxCount = 50);
}
