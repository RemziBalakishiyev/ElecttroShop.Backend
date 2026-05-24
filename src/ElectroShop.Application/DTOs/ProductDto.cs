namespace ElectroShop.Application.DTOs;

/// <summary>
/// Product Data Transfer Object
/// </summary>
public record ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public Guid BrandId { get; init; }
    public string BrandName { get; init; } = string.Empty;
    public decimal VatRate { get; init; }
    public int Stock { get; init; }
    public bool IsActive { get; init; }
    public List<ProductImageDto> Images { get; init; } = [];
    public string? PrimaryImageUrl { get; init; }
    public bool IsBanner { get; init; }
    public bool IsFeatured { get; init; }
    public int? DisplayOrder { get; init; }
    /// <summary>
    /// Final endirim faizi (0-100 arası)
    /// Prioritet: Məhsul > Brand > Kateqoriya
    /// </summary>
    public decimal FinalDiscountPercent { get; init; }
    /// <summary>
    /// Endirimli final qiymət
    /// </summary>
    public decimal FinalPrice { get; init; }
    public List<CategoryAttributeDto> CategoryAttributes { get; init; } = [];
    public List<ProductVariantDto> Variants { get; init; } = [];
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    /// <summary>
    /// RowVersion - Optimistic Concurrency Control üçün
    /// Client-dən gəlir və update zamanı yenidən set edilir
    /// </summary>
    public uint RowVersion { get; init; }
}

/// <summary>
/// Product List DTO (lighter version for list operations)
/// </summary>
public record ProductListDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
    public string CategoryName { get; init; } = string.Empty;
    public string BrandName { get; init; } = string.Empty;
    public int Stock { get; init; }
    public bool IsActive { get; init; }
    public string? PrimaryImageUrl { get; init; }
    public bool IsBanner { get; init; }
    public bool IsFeatured { get; init; }
    public int? DisplayOrder { get; init; }
    /// <summary>
    /// Final endirim faizi (0-100 arası)
    /// </summary>
    public decimal FinalDiscountPercent { get; init; }
    /// <summary>
    /// Endirimli final qiymət
    /// </summary>
    public decimal FinalPrice { get; init; }
}

/// <summary>
/// Create Product DTO
/// </summary>
public record CreateProductDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; } = "AZN";
    public string Sku { get; init; } = string.Empty;
    public Guid CategoryId { get; init; }
    public Guid BrandId { get; init; }
    public decimal VatRate { get; init; } = 0.18m;
    public int Stock { get; init; }
}

/// <summary>
/// Update Product DTO
/// </summary>
public record UpdateProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; } = "AZN";
    public Guid CategoryId { get; init; }
    public Guid BrandId { get; init; }
    public decimal VatRate { get; init; } = 0.18m;
    public int Stock { get; init; }
    public List<Guid> ImageIds { get; init; } = [];
}

/// <summary>
/// Product Image DTO
/// </summary>
public record ProductImageDto
{
    public Guid Id { get; init; }
    public Guid ImageId { get; init; }
    public string? ImageUrl { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsPrimary { get; init; }
}

/// <summary>
/// Category Attribute DTO
/// </summary>
public record CategoryAttributeDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string AttributeType { get; init; } = string.Empty;
    public bool IsRequired { get; init; }
    public int DisplayOrder { get; init; }
    public List<CategoryAttributeValueDto> Values { get; init; } = [];
}

/// <summary>
/// Category Attribute Value DTO
/// </summary>
public record CategoryAttributeValueDto
{
    public Guid Id { get; init; }
    public string Value { get; init; } = string.Empty;
    public string? DisplayValue { get; init; }
    public int DisplayOrder { get; init; }
    public string? ColorCode { get; init; }
}

/// <summary>
/// Product Variant DTO
/// </summary>
public record ProductVariantDto
{
    public Guid Id { get; init; }
    public string Sku { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Currency { get; init; } = string.Empty;
    public int Stock { get; init; }
    public bool IsActive { get; init; }
    public Guid? ImageId { get; init; }
    public string? ImageUrl { get; init; }
    public Dictionary<string, string> Attributes { get; init; } = new();
    public decimal FinalDiscountPercent { get; init; }
    public decimal FinalPrice { get; init; }
}

