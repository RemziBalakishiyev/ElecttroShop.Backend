using ElectroShop.Domain.Enums;

namespace ElectroShop.Application.DTOs;

/// <summary>
/// Discount Data Transfer Object
/// </summary>
public record DiscountDto
{
    public Guid Id { get; init; }
    public DiscountType Type { get; init; }
    public Guid? ProductId { get; init; }
    public string? ProductName { get; init; }
    public Guid? BrandId { get; init; }
    public string? BrandName { get; init; }
    public Guid? CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public decimal Percent { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// Discount List DTO
/// </summary>
public record DiscountListDto
{
    public Guid Id { get; init; }
    public DiscountType Type { get; init; }
    public string TargetName { get; init; } = string.Empty; // Məhsul/Brend/Kateqoriya adı
    public decimal Percent { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public bool IsActive { get; init; }
}

/// <summary>
/// Create Discount DTO
/// </summary>
public record CreateDiscountDto
{
    public DiscountType Type { get; init; }
    public Guid? ProductId { get; init; }
    public Guid? BrandId { get; init; }
    public Guid? CategoryId { get; init; }
    public decimal Percent { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}

/// <summary>
/// Update Discount DTO
/// </summary>
public record UpdateDiscountDto
{
    public decimal Percent { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public bool IsActive { get; init; }
}



