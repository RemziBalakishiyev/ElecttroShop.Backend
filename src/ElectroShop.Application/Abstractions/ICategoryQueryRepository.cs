using ElectroShop.Domain.Entities;

namespace ElectroShop.Application.Abstractions;

/// <summary>
/// Category-specific query repository
/// </summary>
public interface ICategoryQueryRepository : IQueryRepository<Category>
{
    /// <summary>
    /// Səhifələnmiş kateqoriya siyahısı
    /// </summary>
    Task<(List<Category> Categories, int TotalCount)> GetCategoriesPagedAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        Guid? parentId = null,
        bool includeChildren = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Root kateqoriyalar (parent-i olmayan)
    /// </summary>
    Task<List<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
}

