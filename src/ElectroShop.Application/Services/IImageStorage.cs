namespace ElectroShop.Application.Services;

/// <summary>
/// Şəkil saxlama servisi interfeysi (SOLID: Interface Segregation)
/// </summary>
public interface IImageStorage
{
    /// <summary>
    /// Şəkil yükləyir və unikal ID qaytarır
    /// </summary>
    /// <param name="imageStream">Şəkil stream-i</param>
    /// <param name="fileName">Fayl adı</param>
    /// <param name="contentType">Content type (MIME type)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Yüklənmiş şəkilin unikal ID-si</returns>
    Task<Guid> UploadImageAsync(
        Stream imageStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Şəkili silir
    /// </summary>
    /// <param name="imageId">Şəkil ID-si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteImageAsync(Guid imageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Şəkili oxuyur
    /// </summary>
    /// <param name="imageId">Şəkil ID-si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Şəkil stream-i və content type</returns>
    Task<(Stream Stream, string ContentType)?> GetImageAsync(
        Guid imageId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Şəklin mövcud olub-olmadığını yoxlayır
    /// </summary>
    /// <param name="imageId">Şəkil ID-si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<bool> ImageExistsAsync(Guid imageId, CancellationToken cancellationToken = default);
}




