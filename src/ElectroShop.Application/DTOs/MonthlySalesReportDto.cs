namespace ElectroShop.Application.DTOs;

public record MonthlySalesReportDto
{
    public int Year { get; init; }
    public int Month { get; init; }
    public string MonthName { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public DateTime GeneratedAt { get; init; }
    public DateTime ReportDate { get; init; }
    public MonthlySalesReportSummaryDto Summary { get; init; } = new();
    public List<DailySalesReportDto> DailySales { get; init; } = [];
    public List<TopProductReportDto> TopProducts { get; init; } = [];
    public List<CategorySalesReportDto> CategorySales { get; init; } = [];
    public List<SaleTypeReportDto> SaleTypeBreakdown { get; init; } = [];
    public List<ProfitLossProductReportDto> ProfitLossProducts { get; init; } = [];
    public List<MonthlySalesReportItemDto> RecentSales { get; init; } = [];
    public List<MonthlySalesReportItemDto> Items { get; init; } = [];
}

public record MonthlySalesReportSummaryDto
{
    public int SalesCount { get; init; }
    public decimal TotalQuantity { get; init; }
    public decimal TotalSalesAmount { get; init; }
    public decimal TotalCostAmount { get; init; }
    public decimal TotalExpenses { get; init; }
    public decimal GrossProfit { get; init; }
    public decimal NetProfit { get; init; }
    public decimal AverageSaleAmount { get; init; }
    public decimal ProfitMarginPercent { get; init; }
}

public record DailySalesReportDto
{
    public DateTime Date { get; init; }
    public string DayLabel { get; init; } = string.Empty;
    public int SalesCount { get; init; }
    public decimal TotalSalesAmount { get; init; }
    public decimal TotalExpenses { get; init; }
    public decimal GrossProfit { get; init; }
    public decimal NetProfit { get; init; }
}

public record TopProductReportDto
{
    public string ProductName { get; init; } = string.Empty;
    public string? Sku { get; init; }
    public string? CategoryName { get; init; }
    public decimal Quantity { get; init; }
    public decimal TotalSalesAmount { get; init; }
    public decimal TotalProfit { get; init; }
}

public record CategorySalesReportDto
{
    public string CategoryName { get; init; } = string.Empty;
    public int SalesCount { get; init; }
    public decimal Quantity { get; init; }
    public decimal TotalSalesAmount { get; init; }
    public decimal TotalProfit { get; init; }
}

public record SaleTypeReportDto
{
    public string SaleType { get; init; } = string.Empty;
    public int SalesCount { get; init; }
    public decimal TotalSalesAmount { get; init; }
    public decimal TotalProfit { get; init; }
}

public record ProfitLossProductReportDto
{
    public string ProductName { get; init; } = string.Empty;
    public string? Sku { get; init; }
    public decimal TotalSalesAmount { get; init; }
    public decimal TotalCostAmount { get; init; }
    public decimal TotalExpenses { get; init; }
    public decimal NetProfit { get; init; }
    public decimal ProfitMarginPercent { get; init; }
}

public record MonthlySalesReportItemDto
{
    public string ProductName { get; init; } = string.Empty;
    public string? ProductCode { get; init; }
    public string? Sku { get; init; }
    public string? CategoryName { get; init; }
    public string SaleType { get; init; } = string.Empty;
    public decimal SalePrice { get; init; }
    public decimal Quantity { get; init; }
    public decimal TotalCostAmount { get; init; }
    public decimal TotalSalesAmount { get; init; }
    public decimal TotalExpenses { get; init; }
    public decimal GrossProfit { get; init; }
    public decimal NetProfit { get; init; }
    public decimal Profit { get; init; }
    public DateTime SaleDate { get; init; }
}

public record SalesExportFileDto
{
    public byte[] Content { get; init; } = [];
    public string ContentType { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
}
