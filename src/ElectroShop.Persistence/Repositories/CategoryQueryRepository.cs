using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Filtering;
using ElectroShop.Domain.Entities;
using ElectroShop.Persistence.Contexts;
using ElectroShop.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ElectroShop.Persistence.Repositories;

public class CategoryQueryRepository : QueryRepository<Category>, ICategoryQueryRepository
{
    public CategoryQueryRepository(ElectroShopDbContext context) : base(context)
    {
    }

    public async Task<(List<Category> Categories, int TotalCount)> GetCategoriesPagedAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        Guid? parentId = null,
        bool includeChildren = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Include(c => c.Parent).AsNoTracking();

        if (includeChildren)
            query = query.Include(c => c.Children);

        var search = searchTerm?.ToLower();

        var predicate = PredicateBuilder.True<Category>()
            .And(c => !c.IsDeleted)
            .AndIf(!string.IsNullOrWhiteSpace(search), c => c.Name.ToLower().Contains(search!))
            .AndIf(parentId.HasValue, c => c.ParentId == parentId!.Value)
            .AndIf(!parentId.HasValue, c => c.ParentId == null);

        return await QueryHelper.ExecutePagedAsync(
            query.Where(predicate),
            page,
            pageSize,
            c => c.Name,
            descending: false,
            cancellationToken);
    }

    public async Task<List<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.ParentId == null && !c.IsDeleted)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }
}


