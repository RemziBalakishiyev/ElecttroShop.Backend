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

    /// <summary>
    /// Banner məhsulu əldə et
    /// </summary>
    Task<Product?> GetBannerProductAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Featured məhsulları əldə et (DisplayOrder-a görə sıralanmış)
    /// </summary>
    Task<List<Product>> GetFeaturedProductsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Popular məhsulları əldə et (PopularDisplayOrder-a görə sıralanmış, maksimum 4)
    /// </summary>
    Task<List<Product>> GetPopularProductsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Brend üçün featured məhsul əldə et (DisplayOrder-a görə sıralanmış, ilk məhsul seçilir)
    /// </summary>
    Task<Product?> GetFeaturedProductByBrandIdAsync(Guid brandId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Məhsulu şəkillər və variantlarla birlikdə əldə et (Update üçün)
    /// </summary>
    Task<Product?> GetProductWithImagesAndVariantsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// DB-də olan ProductImage-ləri tracked collection-a əlavə edir (Include cartesian bug fix)
    /// </summary>
    Task EnsureProductImagesAttachedAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// ProductImages-i silir (Update üçün - EF Core tracking)
    /// </summary>
    Task DeleteProductImagesByIdsAsync(List<Guid> imageIds, CancellationToken cancellationToken = default);
}

