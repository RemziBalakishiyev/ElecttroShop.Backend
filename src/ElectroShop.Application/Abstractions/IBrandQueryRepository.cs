using ElectroShop.Domain.Entities;

namespace ElectroShop.Application.Abstractions;

/// <summary>
/// Brand-specific query repository
/// </summary>
public interface IBrandQueryRepository : IQueryRepository<Brand>
{
    /// <summary>
    /// Səhifələnmiş brend siyahısı
    /// </summary>
    Task<(List<Brand> Brands, int TotalCount)> GetBrandsPagedAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);
}

