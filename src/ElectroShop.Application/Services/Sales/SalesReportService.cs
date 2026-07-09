using ElectroShop.Application.Abstractions;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;

namespace ElectroShop.Application.Services.Sales;

public class SalesReportService : ISalesReportService
{
    private readonly ISaleQueryRepository _saleQueryRepository;

    public SalesReportService(ISaleQueryRepository saleQueryRepository)
    {
        _saleQueryRepository = saleQueryRepository;
    }

    public async Task<MonthlySalesReportDto> BuildMonthlyReportAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var (startUtc, endUtcExclusive) = SalesMonthHelper.GetMonthRangeUtc(year, month);

        var sales = await _saleQueryRepository.GetSalesBySoldAtRangeAsync(
            startUtc, endUtcExclusive, cancellationToken);

        var stats = await _saleQueryRepository.GetSalesStatisticsAsync(
            startUtc, endUtcExclusive, cancellationToken);

        return new MonthlySalesReportDto
        {
            Year = year,
            Month = month,
            MonthName = SalesMonthHelper.GetDisplayMonthName(month),
            ReportDate = DateTime.UtcNow,
            Summary = MapSummary(stats),
            Items = sales.Select(MapItem).ToList()
        };
    }

    private static MonthlySalesReportSummaryDto MapSummary(SalesStatisticsDto stats) => new()
    {
        SalesCount = stats.SalesCount,
        TotalQuantity = stats.SoldProductQuantity,
        TotalSalesAmount = stats.TotalSaleAmount,
        TotalCostAmount = stats.TotalProductCost,
        TotalExpenses = stats.TotalExpenses,
        GrossProfit = stats.TotalSaleAmount - stats.TotalProductCost,
        NetProfit = stats.TotalProfit
    };

    private static MonthlySalesReportItemDto MapItem(Sale sale) => new()
    {
        ProductName = sale.ProductName,
        ProductCode = sale.ProductCode,
        Sku = sale.ProductCode,
        CategoryName = sale.CategoryName,
        SaleType = SaleSourceDisplayHelper.ToDisplayName(sale.SaleSource),
        SalePrice = sale.SalePrice,
        Quantity = sale.Quantity,
        TotalCostAmount = sale.TotalCost,
        TotalSalesAmount = sale.TotalSaleAmount,
        TotalExpenses = sale.TotalExpenses,
        Profit = sale.Profit,
        SaleDate = sale.SoldAt
    };
}
