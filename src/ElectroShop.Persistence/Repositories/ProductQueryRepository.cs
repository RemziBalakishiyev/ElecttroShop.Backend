using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Filtering;
using ElectroShop.Domain.Entities;
using ElectroShop.Persistence.Contexts;
using ElectroShop.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ElectroShop.Persistence.Repositories;

public class ProductQueryRepository : QueryRepository<Product>, IProductQueryRepository
{
    public ProductQueryRepository(ElectroShopDbContext context) : base(context)
    {
    }

    public async Task<(List<Product> Products, int TotalCount)> GetProductsPagedAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        Guid? categoryId = null,
        Guid? brandId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.ProductImages.OrderBy(pi => pi.DisplayOrder))
            .AsNoTracking();

        var search = searchTerm?.ToLower();
        
        var predicate = PredicateBuilder.True<Product>()
            .And(p => !p.IsDeleted)
            .AndIf(!string.IsNullOrWhiteSpace(search), p =>
                p.Name.ToLower().Contains(search!) ||
                (p.Description != null && p.Description.ToLower().Contains(search!)) ||
                p.Sku.Value.ToLower().Contains(search!))
            .AndIf(categoryId.HasValue, p => p.CategoryId == categoryId!.Value)
            .AndIf(brandId.HasValue, p => p.BrandId == brandId!.Value)
            .AndIf(minPrice.HasValue, p => p.Price.Amount >= minPrice!.Value)
            .AndIf(maxPrice.HasValue, p => p.Price.Amount <= maxPrice!.Value)
            .AndIf(isActive.HasValue, p => p.IsActive == isActive!.Value);

        return await QueryHelper.ExecutePagedAsync(
            query.Where(predicate),
            page,
            pageSize,
            p => p.CreatedAtUtc,
            descending: true,
            cancellationToken);
    }

    public async Task<Product?> GetProductWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.ProductImages.OrderBy(pi => pi.DisplayOrder))
            .Include(p => p.ProductVariants.Where(pv => pv.IsActive && !pv.IsDeleted))
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
    }

    public async Task<Product?> GetBannerProductAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.ProductImages.OrderBy(pi => pi.DisplayOrder))
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IsBanner && !p.IsDeleted && p.IsActive, cancellationToken);
    }

    public async Task<List<Product>> GetFeaturedProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.ProductImages.OrderBy(pi => pi.DisplayOrder))
            .AsNoTracking()
            .Where(p => p.IsFeatured && !p.IsDeleted && p.IsActive)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Product>> GetPopularProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.ProductImages.OrderBy(pi => pi.DisplayOrder))
            .AsNoTracking()
            .Where(p => p.IsPopular && !p.IsDeleted && p.IsActive)
            .OrderBy(p => p.PopularDisplayOrder)
            .Take(4)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetFeaturedProductByBrandIdAsync(Guid brandId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.ProductImages.OrderBy(pi => pi.DisplayOrder))
            .AsNoTracking()
            .Where(p => p.BrandId == brandId && p.IsFeatured && !p.IsDeleted && p.IsActive)
            .OrderBy(p => p.DisplayOrder)
            .ThenBy(p => p.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Product?> GetProductWithImagesAndVariantsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsSplitQuery()
            .Include(p => p.ProductImages.OrderBy(pi => pi.DisplayOrder))
            .Include(p => p.ProductVariants)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
    }

    public async Task EnsureProductImagesAttachedAsync(Product product, CancellationToken cancellationToken = default)
    {
        var trackedIds = product.ProductImages.Select(pi => pi.Id).ToHashSet();
        var productImageDbSet = _context.Set<ProductImage>();

        var missingImages = await productImageDbSet
            .Where(pi => pi.ProductId == product.Id && !trackedIds.Contains(pi.Id))
            .ToListAsync(cancellationToken);

        foreach (var image in missingImages)
        {
            productImageDbSet.Attach(image);
            product.ProductImages.Add(image);
        }
    }

    public async Task DeleteProductImagesByIdsAsync(List<Guid> imageIds, CancellationToken cancellationToken = default)
    {
        if (imageIds == null || imageIds.Count == 0)
            return;

        // ProductImages-i ID-lərə görə tap və sil (EF Core tracking üçün)
        var productImageDbSet = _context.Set<Domain.Entities.ProductImage>();
        var imagesToDelete = await productImageDbSet
            .Where(pi => imageIds.Contains(pi.Id))
            .ToListAsync(cancellationToken);
        
        if (imagesToDelete.Count > 0)
        {
            productImageDbSet.RemoveRange(imagesToDelete);
        }
    }
}




