using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Filtering;
using ElectroShop.Domain.Entities;
using ElectroShop.Persistence.Contexts;
using ElectroShop.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ElectroShop.Persistence.Repositories;

public class BrandQueryRepository : QueryRepository<Brand>, IBrandQueryRepository
{
    public BrandQueryRepository(ElectroShopDbContext context) : base(context)
    {
    }

    public async Task<(List<Brand> Brands, int TotalCount)> GetBrandsPagedAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking();
        var search = searchTerm?.ToLower();

        var predicate = PredicateBuilder.True<Brand>()
            .And(b => !b.IsDeleted)
            .AndIf(!string.IsNullOrWhiteSpace(search), b => b.Name.ToLower().Contains(search!));

        return await QueryHelper.ExecutePagedAsync(
            query.Where(predicate),
            page,
            pageSize,
            b => b.Name,
            descending: false,
            cancellationToken);
    }

    public async Task<List<Brand>> GetPromotionalBrandsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(b => b.IsPromotional && !b.IsDeleted)
            .OrderBy(b => b.DisplayOrder)
            .ThenBy(b => b.CreatedAtUtc)
            .Take(4)
            .ToListAsync(cancellationToken);
    }
}


