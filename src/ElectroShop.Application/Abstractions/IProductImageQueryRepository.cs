namespace ElectroShop.Application.Abstractions;

public interface IProductImageQueryRepository
{
    Task<ProductImageReferenceDto?> GetByImageIdAsync(
        Guid imageId,
        CancellationToken cancellationToken = default);
}

public record ProductImageReferenceDto(
    Guid Id,
    Guid ProductId,
    Guid ImageId,
    string? ImageUrl = null,
    string? PublicId = null,
    string? ImagePath = null,
    string? FileName = null,
    string? ContentType = null,
    string? StorageProvider = null);
