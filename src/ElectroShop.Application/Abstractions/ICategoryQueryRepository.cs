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
        bool includeAll = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Root kateqoriyalar (parent-i olmayan)
    /// </summary>
    Task<List<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Kateqoriya atributlarını əldə et (dəyərlərlə birlikdə)
    /// </summary>
    Task<List<CategoryAttribute>> GetCategoryAttributesAsync(Guid categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kateqoriya atributunu dəyərlərlə birlikdə əldə et (AsNoTracking - read-only)
    /// </summary>
    Task<CategoryAttribute?> GetCategoryAttributeWithValuesAsync(Guid attributeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kateqoriya atributunu dəyərlərlə birlikdə əldə et (tracking ilə - update üçün)
    /// </summary>
    Task<CategoryAttribute?> GetCategoryAttributeWithValuesForUpdateAsync(Guid attributeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Value ID-sinə görə atribut və dəyəri əldə et (AsNoTracking - read-only)
    /// </summary>
    Task<(CategoryAttribute Attribute, CategoryAttributeValue Value)?> GetAttributeAndValueByValueIdAsync(Guid valueId, CancellationToken cancellationToken = default);

    /// <summary>
    /// CategoryAttributeValue'yu tracking ilə əldə et (update üçün)
    /// </summary>
    Task<CategoryAttributeValue?> GetCategoryAttributeValueForUpdateAsync(Guid valueId, CancellationToken cancellationToken = default);

    /// <summary>
    /// CategoryAttributeValue əlavə et
    /// </summary>
    Task AddCategoryAttributeValueAsync(CategoryAttributeValue value, CancellationToken cancellationToken = default);

    /// <summary>
    /// CategoryAttributeValue güncəllə
    /// </summary>
    void UpdateCategoryAttributeValue(CategoryAttributeValue value);

    /// <summary>
    /// Kateqoriya atributlarını tracking ilə əldə et (upsert üçün)
    /// </summary>
    Task<List<CategoryAttribute>> GetCategoryAttributesForUpdateAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default);
}

