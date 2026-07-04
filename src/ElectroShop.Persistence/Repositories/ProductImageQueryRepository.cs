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
            productImage.ImageId);
    }
}
