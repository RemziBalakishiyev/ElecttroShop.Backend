using ElectroShop.Domain.Enums;

namespace ElectroShop.Application.Abstractions;

public record DailySalesAggregateDto
{
    public DateTime Date { get; init; }
    public int SalesCount { get; init; }
    public decimal TotalSalesAmount { get; init; }
    public decimal TotalCostAmount { get; init; }
    public decimal TotalExpenses { get; init; }
}

public record TopProductAggregateDto
{
    public string ProductName { get; init; } = string.Empty;
    public string? Sku { get; init; }
    public string? CategoryName { get; init; }
    public decimal Quantity { get; init; }
    public decimal TotalSalesAmount { get; init; }
    public decimal TotalProfit { get; init; }
}

public record CategorySalesAggregateDto
{
    public string CategoryName { get; init; } = string.Empty;
    public int SalesCount { get; init; }
    public decimal Quantity { get; init; }
    public decimal TotalSalesAmount { get; init; }
    public decimal TotalProfit { get; init; }
}

public record SaleTypeAggregateDto
{
    public SaleSource SaleSource { get; init; }
    public int SalesCount { get; init; }
    public decimal TotalSalesAmount { get; init; }
    public decimal TotalProfit { get; init; }
}

public record ProductProfitAggregateDto
{
    public string ProductName { get; init; } = string.Empty;
    public string? Sku { get; init; }
    public decimal TotalSalesAmount { get; init; }
    public decimal TotalCostAmount { get; init; }
    public decimal TotalExpenses { get; init; }
    public decimal NetProfit { get; init; }
}

public record RecentSaleAggregateDto
{
    public string ProductName { get; init; } = string.Empty;
    public string? ProductCode { get; init; }
    public string? CategoryName { get; init; }
    public SaleSource SaleSource { get; init; }
    public decimal SalePrice { get; init; }
    public decimal Quantity { get; init; }
    public decimal TotalCost { get; init; }
    public decimal TotalSaleAmount { get; init; }
    public decimal TotalExpenses { get; init; }
    public decimal Profit { get; init; }
    public DateTime SoldAt { get; init; }
}
