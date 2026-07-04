using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Dashboard.Queries.GetDashboardStatistics;

/// <summary>
/// Admin dashboard satış və məhsul statistikalarını hesablayır
/// </summary>
public class GetDashboardStatisticsQueryHandler
    : IRequestHandler<GetDashboardStatisticsQuery, Result<DashboardStatisticsResponse>>
{
    private readonly ISaleQueryRepository _saleQueryRepository;
    private readonly IProductQueryRepository _productQueryRepository;

    public GetDashboardStatisticsQueryHandler(
        ISaleQueryRepository saleQueryRepository,
        IProductQueryRepository productQueryRepository)
    {
        _saleQueryRepository = saleQueryRepository;
        _productQueryRepository = productQueryRepository;
    }

    public async Task<Result<DashboardStatisticsResponse>> Handle(
        GetDashboardStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        var nowUtc = DateTime.UtcNow;

        var startOfTodayUtc = new DateTime(nowUtc.Year, nowUtc.Month, nowUtc.Day, 0, 0, 0, DateTimeKind.Utc);
        var startOfTomorrowUtc = startOfTodayUtc.AddDays(1);

        var startOfMonthUtc = new DateTime(nowUtc.Year, nowUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endOfTodayExclusiveUtc = startOfTomorrowUtc;

        var dailyStats = await _saleQueryRepository.GetSalesStatisticsAsync(
            startOfTodayUtc,
            startOfTomorrowUtc,
            cancellationToken);

        var monthlyStats = await _saleQueryRepository.GetSalesStatisticsAsync(
            startOfMonthUtc,
            endOfTodayExclusiveUtc,
            cancellationToken);

        var productSummary = await _productQueryRepository.GetProductSummaryStatisticsAsync(cancellationToken);

        var response = new DashboardStatisticsResponse
        {
            DailySales = MapSalesStatistics(dailyStats),
            MonthlySales = MapSalesStatistics(monthlyStats),
            ProductSummary = new ProductSummaryStatisticsResponse
            {
                TotalProductCount = productSummary.TotalProductCount,
                TotalProductCostValue = productSummary.TotalProductCostValue,
                TotalProductSaleValue = productSummary.TotalProductSaleValue,
                TotalInventoryCostValue = productSummary.TotalInventoryCostValue,
                TotalInventorySaleValue = productSummary.TotalInventorySaleValue
            }
        };

        return Result.Success(response);
    }

    private static SalesStatisticsResponse MapSalesStatistics(SalesStatisticsDto stats) =>
        new()
        {
            TotalSaleAmount = stats.TotalSaleAmount,
            TotalProductCost = stats.TotalProductCost,
            TotalExpenses = stats.TotalExpenses,
            TotalProfit = stats.TotalProfit,
            SoldProductQuantity = stats.SoldProductQuantity,
            SalesCount = stats.SalesCount
        };
}
