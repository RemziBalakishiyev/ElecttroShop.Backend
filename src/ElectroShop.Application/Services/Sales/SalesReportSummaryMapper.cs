using ElectroShop.Application.Abstractions;
using ElectroShop.Application.DTOs;

namespace ElectroShop.Application.Services.Sales;

internal static class SalesReportSummaryMapper
{
    public static MonthlySalesReportSummaryDto MapSummary(SalesStatisticsDto stats)
    {
        var grossProfit = stats.TotalSaleAmount - stats.TotalProductCost;
        var netProfit = stats.TotalProfit;

        return new MonthlySalesReportSummaryDto
        {
            SalesCount = stats.SalesCount,
            TotalQuantity = stats.SoldProductQuantity,
            TotalSalesAmount = stats.TotalSaleAmount,
            TotalCostAmount = stats.TotalProductCost,
            TotalExpenses = stats.TotalExpenses,
            GrossProfit = grossProfit,
            NetProfit = netProfit,
            AverageSaleAmount = stats.SalesCount > 0
                ? stats.TotalSaleAmount / stats.SalesCount
                : 0,
            ProfitMarginPercent = stats.TotalSaleAmount > 0
                ? netProfit / stats.TotalSaleAmount * 100
                : 0
        };
    }
}
