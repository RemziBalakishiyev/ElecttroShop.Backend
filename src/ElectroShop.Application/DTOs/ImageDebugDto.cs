namespace ElectroShop.Application.DTOs;

public record UploadsDebugResponse
{
    public string WebRootPath { get; init; } = string.Empty;
    public string ContentRootPath { get; init; } = string.Empty;
    public string ImageStorageBasePath { get; init; } = string.Empty;
    public bool UploadsFolderExists { get; init; }
    public int FileCount { get; init; }
    public IReadOnlyList<UploadDebugFileDto> Files { get; init; } = [];
}

public record UploadDebugFileDto
{
    public string FileName { get; init; } = string.Empty;
    public string RelativePath { get; init; } = string.Empty;
    public string PublicUrl { get; init; } = string.Empty;
    public long Size { get; init; }
}

public record ImageDebugResponse
{
    public Guid ImageId { get; init; }
    public bool ImageRecordFound { get; init; }
    public Guid? ProductImageId { get; init; }
    public Guid? ProductId { get; init; }
    public string? ImageUrl { get; init; }
    public string? PublicId { get; init; }
    public string? ImagePath { get; init; }
    public string? StorageProvider { get; init; }
    public string? StoredPath { get; init; }
    public string? StoredFileName { get; init; }
    public string PhysicalPathSearched { get; init; } = string.Empty;
    public bool FileExists { get; init; }
    public string? DetectedExtension { get; init; }
    public string? PublicUrl { get; init; }
    public string? StaticPublicUrl { get; init; }
}
