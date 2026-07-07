using ElectroShop.Application.Abstractions;
using ElectroShop.Domain.Entities;
using ElectroShop.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ElectroShop.Persistence.Repositories;

public class ProductImageQueryRepository : IProductImageQueryRepository
{
    private readonly ElectroShopDbContext _context;

    public ProductImageQueryRepository(ElectroShopDbContext context)
    {
        _context = context;
    }

    public async Task<ProductImageReferenceDto?> GetByImageIdAsync(
        Guid imageId,
        CancellationToken cancellationToken = default)
    {
        var productImage = await _context.Set<ProductImage>()
            .AsNoTracking()
            .FirstOrDefaultAsync(pi => pi.ImageId == imageId, cancellationToken);

        if (productImage == null)
            return null;

        return new ProductImageReferenceDto(
            productImage.Id,
            productImage.ProductId,
            productImage.ImageId,
            productImage.ImageUrl,
            productImage.PublicId,
            productImage.ImagePath,
            productImage.FileName,
            productImage.ContentType,
            productImage.StorageProvider);
    }

    public async Task<IReadOnlyList<ProductImage>> GetImagesNeedingCloudinaryBackfillAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<ProductImage>()
            .Where(pi =>
                string.IsNullOrEmpty(pi.ImageUrl) ||
                pi.StorageProvider != "Cloudinary")
            .OrderBy(pi => pi.Id)
            .ToListAsync(cancellationToken);
    }
}
