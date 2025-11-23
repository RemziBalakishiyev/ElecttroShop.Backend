namespace ElectroShop.Application.DTOs;

/// <summary>
/// Category Data Transfer Object
/// </summary>
public record CategoryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Slug { get; init; }
    public Guid? ParentId { get; init; }
    public string? ParentName { get; init; }
    /// <summary>
    /// Bu kateqoriyaya tətbiq olunan endirim faizi (0-100 arası)
    /// </summary>
    public decimal DiscountPercent { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Category List DTO
/// </summary>
public record CategoryListDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Slug { get; init; }
    /// <summary>
    /// Bu kateqoriyaya tətbiq olunan endirim faizi (0-100 arası)
    /// </summary>
    public decimal DiscountPercent { get; init; }
}

