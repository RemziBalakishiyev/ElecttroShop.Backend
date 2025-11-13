using ElectroShop.Application.Common.Results;
using ElectroShop.Domain.Entities;

namespace ElectroShop.Application.Abstractions;

/// <summary>
/// Product-specific query repository
/// LINQ və Include-lar Repository-də (Infrastructure layer)
/// </summary>
public interface IProductQueryRepository : IQueryRepository<Product>
{
    /// <summary>
    /// Səhifələnmiş məhsul siyahısı - bütün filtering və include SQL-də
    /// </summary>
    Task<(List<Product> Products, int TotalCount)> GetProductsPagedAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        Guid? categoryId = null,
        Guid? brandId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ID ilə məhsul + navigation properties
    /// SQL: SELECT p.*, c.*, b.* ... INNER JOIN ...
    /// </summary>
    Task<Product?> GetProductWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}

