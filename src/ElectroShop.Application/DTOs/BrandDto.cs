namespace ElectroShop.Application.DTOs;

/// <summary>
/// Brand Data Transfer Object
/// </summary>
public record BrandDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    /// <summary>
    /// Bu brendə tətbiq olunan endirim faizi (0-100 arası)
    /// </summary>
    public decimal DiscountPercent { get; init; }
    /// <summary>
    /// Brend promotional olub-olmadığı
    /// </summary>
    public bool IsPromotional { get; init; }
    /// <summary>
    /// Promotional brendlərin sıralaması
    /// </summary>
    public int? DisplayOrder { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Promotional Brand with Featured Product DTO
/// </summary>
public record PromotionalBrandDto
{
    public BrandInfoDto Brand { get; init; } = default!;
    public ProductDto FeaturedProduct { get; init; } = default!;
}

/// <summary>
/// Brand Info DTO for Promotional Brands
/// </summary>
public record BrandInfoDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    /// <summary>
    /// Bu brendə tətbiq olunan endirim faizi (0-100 arası)
    /// </summary>
    public decimal DiscountPercent { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Brand List DTO
/// </summary>
public record BrandListDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    /// <summary>
    /// Bu brendə tətbiq olunan endirim faizi (0-100 arası)
    /// </summary>
    public decimal DiscountPercent { get; init; }
    /// <summary>
    /// Brend promotional olub-olmadığı
    /// </summary>
    public bool IsPromotional { get; init; }
    /// <summary>
    /// Promotional brendlərin sıralaması
    /// </summary>
    public int? DisplayOrder { get; init; }
}

