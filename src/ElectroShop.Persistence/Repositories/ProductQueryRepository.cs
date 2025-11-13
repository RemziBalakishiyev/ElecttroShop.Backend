using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Filtering;
using ElectroShop.Domain.Entities;
using ElectroShop.Persistence.Contexts;
using ElectroShop.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;

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
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
    }
}




