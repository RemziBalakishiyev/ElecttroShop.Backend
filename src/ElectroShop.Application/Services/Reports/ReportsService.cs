using ElectroShop.Application.Abstractions;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services.Sales;

namespace ElectroShop.Application.Services.Reports;

public class ReportsService : IReportsService
{
    private const int TopProductsLimit = 10;
    private const int ProfitLossTopCount = 5;
    private const int RecentSalesLimit = 20;

    private readonly ISaleQueryRepository _saleQueryRepository;

    public ReportsService(ISaleQueryRepository saleQueryRepository)
    {
        _saleQueryRepository = saleQueryRepository;
    }

    public async Task<MonthlySalesReportDto> GetMonthlySalesReportAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var (startUtc, endUtcExclusive) = SalesMonthHelper.GetMonthRangeUtc(year, month);
        var generatedAt = DateTime.UtcNow;

        var stats = await _saleQueryRepository.GetSalesStatisticsAsync(
            startUtc, endUtcExclusive, cancellationToken);

        var dailyAggregates = await _saleQueryRepository.GetDailySalesAggregatesAsync(
            startUtc, endUtcExclusive, cancellationToken);

        var topProducts = await _saleQueryRepository.GetTopProductsAsync(
            startUtc, endUtcExclusive, TopProductsLimit, cancellationToken);

        var categorySales = await _saleQueryRepository.GetCategorySalesAggregatesAsync(
            startUtc, endUtcExclusive, cancellationToken);

        var saleTypeAggregates = await _saleQueryRepository.GetSaleTypeAggregatesAsync(
            startUtc, endUtcExclusive, cancellationToken);

        var productProfitAggregates = await _saleQueryRepository.GetProductProfitAggregatesAsync(
            startUtc, endUtcExclusive, cancellationToken);

        var recentSales = await _saleQueryRepository.GetRecentSalesAsync(
            startUtc, endUtcExclusive, RecentSalesLimit, cancellationToken);

        return new MonthlySalesReportDto
        {
            Year = year,
            Month = month,
            MonthName = SalesMonthHelper.GetDisplayMonthName(month),
            StartDate = startUtc,
            EndDate = endUtcExclusive,
            GeneratedAt = generatedAt,
            ReportDate = generatedAt,
            Summary = SalesReportSummaryMapper.MapSummary(stats),
            DailySales = BuildDailySales(year, month, dailyAggregates),
            TopProducts = MapTopProducts(topProducts),
            CategorySales = MapCategorySales(categorySales),
            SaleTypeBreakdown = MapSaleTypeBreakdown(saleTypeAggregates),
            ProfitLossProducts = BuildProfitLossProducts(productProfitAggregates),
            RecentSales = MapRecentSales(recentSales)
        };
    }

    private static List<DailySalesReportDto> BuildDailySales(
        int year,
        int month,
        List<DailySalesAggregateDto> aggregates)
    {
        var aggregateByDate = aggregates.ToDictionary(a => a.Date.Date);
        var daysInMonth = DateTime.DaysInMonth(year, month);
        var monthName = SalesMonthHelper.GetDisplayMonthName(month);
        var dailySales = new List<DailySalesReportDto>(daysInMonth);

        for (var day = 1; day <= daysInMonth; day++)
        {
            var date = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
            if (!aggregateByDate.TryGetValue(date.Date, out var aggregate))
            {
                dailySales.Add(new DailySalesReportDto
                {
                    Date = date,
                    DayLabel = $"{day} {monthName}",
                    SalesCount = 0,
                    TotalSalesAmount = 0,
                    TotalExpenses = 0,
                    GrossProfit = 0,
                    NetProfit = 0
                });
                continue;
            }

            var grossProfit = aggregate.TotalSalesAmount - aggregate.TotalCostAmount;
            var netProfit = aggregate.TotalSalesAmount - aggregate.TotalCostAmount - aggregate.TotalExpenses;

            dailySales.Add(new DailySalesReportDto
            {
                Date = date,
                DayLabel = $"{day} {monthName}",
                SalesCount = aggregate.SalesCount,
                TotalSalesAmount = aggregate.TotalSalesAmount,
                TotalExpenses = aggregate.TotalExpenses,
                GrossProfit = grossProfit,
                NetProfit = netProfit
            });
        }

        return dailySales;
    }

    private static List<TopProductReportDto> MapTopProducts(List<TopProductAggregateDto> aggregates) =>
        aggregates.Select(p => new TopProductReportDto
        {
            ProductName = p.ProductName,
            Sku = p.Sku,
            CategoryName = p.CategoryName,
            Quantity = p.Quantity,
            TotalSalesAmount = p.TotalSalesAmount,
            TotalProfit = p.TotalProfit
        }).ToList();

    private static List<CategorySalesReportDto> MapCategorySales(List<CategorySalesAggregateDto> aggregates) =>
        aggregates.Select(c => new CategorySalesReportDto
        {
            CategoryName = c.CategoryName,
            SalesCount = c.SalesCount,
            Quantity = c.Quantity,
            TotalSalesAmount = c.TotalSalesAmount,
            TotalProfit = c.TotalProfit
        }).ToList();

    private static List<SaleTypeReportDto> MapSaleTypeBreakdown(List<SaleTypeAggregateDto> aggregates) =>
        aggregates.Select(s => new SaleTypeReportDto
        {
            SaleType = SaleSourceDisplayHelper.ToDisplayName(s.SaleSource),
            SalesCount = s.SalesCount,
            TotalSalesAmount = s.TotalSalesAmount,
            TotalProfit = s.TotalProfit
        }).ToList();

    private static List<ProfitLossProductReportDto> BuildProfitLossProducts(
        List<ProductProfitAggregateDto> aggregates)
    {
        if (aggregates.Count == 0)
            return [];

        var topWinners = aggregates
            .OrderByDescending(p => p.NetProfit)
            .Take(ProfitLossTopCount);

        var topLosers = aggregates
            .OrderBy(p => p.NetProfit)
            .Take(ProfitLossTopCount);

        return topWinners
            .Concat(topLosers)
            .DistinctBy(p => new { p.ProductName, p.Sku })
            .Select(MapProfitLossProduct)
            .ToList();
    }

    private static ProfitLossProductReportDto MapProfitLossProduct(ProductProfitAggregateDto aggregate) =>
        new()
        {
            ProductName = aggregate.ProductName,
            Sku = aggregate.Sku,
            TotalSalesAmount = aggregate.TotalSalesAmount,
            TotalCostAmount = aggregate.TotalCostAmount,
            TotalExpenses = aggregate.TotalExpenses,
            NetProfit = aggregate.NetProfit,
            ProfitMarginPercent = aggregate.TotalSalesAmount > 0
                ? aggregate.NetProfit / aggregate.TotalSalesAmount * 100
                : 0
        };

    private static List<MonthlySalesReportItemDto> MapRecentSales(List<RecentSaleAggregateDto> aggregates) =>
        aggregates.Select(s => new MonthlySalesReportItemDto
        {
            ProductName = s.ProductName,
            ProductCode = s.ProductCode,
            Sku = s.ProductCode,
            CategoryName = s.CategoryName,
            SaleType = SaleSourceDisplayHelper.ToDisplayName(s.SaleSource),
            SalePrice = s.SalePrice,
            Quantity = s.Quantity,
            TotalCostAmount = s.TotalCost,
            TotalSalesAmount = s.TotalSaleAmount,
            TotalExpenses = s.TotalExpenses,
            GrossProfit = s.TotalSaleAmount - s.TotalCost,
            NetProfit = s.Profit,
            Profit = s.Profit,
            SaleDate = s.SoldAt
        }).ToList();
}
