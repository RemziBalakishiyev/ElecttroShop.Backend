namespace ElectroShop.Application.DTOs;

public record ImageUploadResultDto
{
    public string? Url { get; init; }
    public string? SecureUrl { get; init; }
    public string PublicId { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long Size { get; init; }
    public string StorageProvider { get; init; } = "Cloudinary";
}
