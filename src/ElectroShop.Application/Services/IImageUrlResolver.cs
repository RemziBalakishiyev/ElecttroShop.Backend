namespace ElectroShop.Application.Services;

public interface IImageUrlResolver
{
    string BuildImageUrl(Guid imageId, string? extension = null);

    Task<string> BuildImageUrlAsync(Guid imageId, CancellationToken cancellationToken = default);

    Task<string> ResolveProductImageUrlAsync(
        Domain.Entities.ProductImage productImage,
        CancellationToken cancellationToken = default);

    string? BuildStaticImageUrl(Guid imageId, string extension);

    string? ResolvePublicUrl(string? value);
}
