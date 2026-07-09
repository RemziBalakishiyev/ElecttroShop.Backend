namespace ElectroShop.Application.DTOs;

public record MonthlySalesReportDto
{
    public int Year { get; init; }
    public int Month { get; init; }
    public string MonthName { get; init; } = string.Empty;
    public DateTime ReportDate { get; init; }
    public MonthlySalesReportSummaryDto Summary { get; init; } = new();
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
    public decimal Profit { get; init; }
    public DateTime SaleDate { get; init; }
}

public record SalesExportFileDto
{
    public byte[] Content { get; init; } = [];
    public string ContentType { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
}
