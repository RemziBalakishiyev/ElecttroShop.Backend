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

    public async Task<List<CategoryAttribute>> GetCategoryAttributesAsync(
        Guid categoryId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.CategoryAttributes
            .Include(ca => ca.Values.OrderBy(cav => cav.DisplayOrder))
            .AsNoTracking()
            .Where(ca => ca.CategoryId == categoryId && !ca.IsDeleted)
            .OrderBy(ca => ca.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<CategoryAttribute?> GetCategoryAttributeWithValuesAsync(
        Guid attributeId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.CategoryAttributes
            .Include(ca => ca.Values.OrderBy(cav => cav.DisplayOrder))
            .AsNoTracking()
            .FirstOrDefaultAsync(ca => ca.Id == attributeId && !ca.IsDeleted, cancellationToken);
    }

    public async Task<(CategoryAttribute Attribute, CategoryAttributeValue Value)?> GetAttributeAndValueByValueIdAsync(
        Guid valueId, 
        CancellationToken cancellationToken = default)
    {
        var attribute = await _context.CategoryAttributes
            .Include(ca => ca.Values)
            .AsNoTracking()
            .FirstOrDefaultAsync(ca => ca.Values.Any(v => v.Id == valueId) && !ca.IsDeleted, cancellationToken);

        if (attribute == null)
            return null;

        var value = attribute.Values.FirstOrDefault(v => v.Id == valueId);
        if (value == null)
            return null;

        return (attribute, value);
    }

    public async Task<CategoryAttribute?> GetCategoryAttributeWithValuesForUpdateAsync(
        Guid attributeId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.CategoryAttributes
            .Include(ca => ca.Values.OrderBy(cav => cav.DisplayOrder))
            .FirstOrDefaultAsync(ca => ca.Id == attributeId && !ca.IsDeleted, cancellationToken);
    }

    public async Task<CategoryAttributeValue?> GetCategoryAttributeValueForUpdateAsync(
        Guid valueId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.CategoryAttributeValues
            .Include(cav => cav.CategoryAttribute)
            .ThenInclude(ca => ca.Values)
            .FirstOrDefaultAsync(cav => cav.Id == valueId, cancellationToken);
    }

    public async Task AddCategoryAttributeValueAsync(
        CategoryAttributeValue value, 
        CancellationToken cancellationToken = default)
    {
        await _context.CategoryAttributeValues.AddAsync(value, cancellationToken);
    }

    public void UpdateCategoryAttributeValue(CategoryAttributeValue value)
    {
        _context.CategoryAttributeValues.Update(value);
    }
}


