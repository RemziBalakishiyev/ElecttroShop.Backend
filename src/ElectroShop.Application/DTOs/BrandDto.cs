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
}

