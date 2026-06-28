using ElectroShop.Domain.Enums;

namespace ElectroShop.Application.DTOs;

public record SaleListItemDto
{
    public Guid Id { get; init; }
    public Guid? ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? ProductCode { get; init; }
    public Guid? CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public decimal CostPrice { get; init; }
    public decimal SalePrice { get; init; }
    public int Quantity { get; init; }
    public decimal TotalCost { get; init; }
    public decimal TotalSaleAmount { get; init; }
    public decimal Profit { get; init; }
    public SaleSource SaleSource { get; init; }
    public DateTime SoldAt { get; init; }
    public string? Note { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record SaleDetailDto : SaleListItemDto
{
    public DateTime? UpdatedAt { get; init; }
    public string? CreatedBy { get; init; }
    public string? UpdatedBy { get; init; }
}
