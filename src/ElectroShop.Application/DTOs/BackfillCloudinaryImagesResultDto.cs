namespace ElectroShop.Application.DTOs;

public record BackfillCloudinaryImagesResultDto
{
    public int TotalCandidates { get; init; }
    public int MigratedCount { get; init; }
    public int SkippedCount { get; init; }
    public int FailedCount { get; init; }
    public IReadOnlyList<BackfillCloudinaryImageItemDto> Migrated { get; init; } = [];
    public IReadOnlyList<BackfillCloudinaryImageItemDto> Skipped { get; init; } = [];
    public IReadOnlyList<BackfillCloudinaryImageItemDto> Failed { get; init; } = [];
}

public record BackfillCloudinaryImageItemDto
{
    public Guid ProductImageId { get; init; }
    public Guid ProductId { get; init; }
    public Guid ImageId { get; init; }
    public string? Reason { get; init; }
    public string? ImageUrl { get; init; }
}
