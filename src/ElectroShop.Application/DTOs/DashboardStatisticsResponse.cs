namespace ElectroShop.Application.DTOs;

/// <summary>
/// Admin dashboard satış və məhsul statistikaları
/// </summary>
public record DashboardStatisticsResponse
{
    public SalesStatisticsResponse DailySales { get; init; } = new();
    public SalesStatisticsResponse MonthlySales { get; init; } = new();
    public ProductSummaryStatisticsResponse ProductSummary { get; init; } = new();
}

/// <summary>
/// Satış statistikaları (günlük və ya aylıq interval üçün)
/// </summary>
public record SalesStatisticsResponse
{
    public decimal TotalSaleAmount { get; init; }
    public decimal TotalProductCost { get; init; }
    public decimal TotalExpenses { get; init; }
    public decimal TotalProfit { get; init; }
    public int SoldProductQuantity { get; init; }
    public int SalesCount { get; init; }
}

/// <summary>
/// Sistemdə mövcud məhsullar üzrə statistikalar
/// </summary>
public record ProductSummaryStatisticsResponse
{
    public int TotalProductCount { get; init; }
    public decimal TotalProductCostValue { get; init; }
    public decimal TotalProductSaleValue { get; init; }
    public decimal TotalInventoryCostValue { get; init; }
    public decimal TotalInventorySaleValue { get; init; }
}
