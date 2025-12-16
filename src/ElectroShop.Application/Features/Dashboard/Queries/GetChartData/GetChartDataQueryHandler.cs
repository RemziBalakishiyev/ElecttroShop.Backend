using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Dashboard.Queries.GetChartData;

/// <summary>
/// Chart məlumatlarını əldə etmək üçün Handler
/// </summary>
public class GetChartDataQueryHandler : IRequestHandler<GetChartDataQuery, Result<ChartDataDto>>
{
    private readonly IOrderQueryRepository _orderRepository;

    public GetChartDataQueryHandler(IOrderQueryRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<ChartDataDto>> Handle(
        GetChartDataQuery request,
        CancellationToken cancellationToken)
    {
        // DbContext thread-safe olmadığı üçün ardıcıl olaraq işlədirik
        var revenueByDate = await _orderRepository.GetRevenueByDateAsync(
            request.Period,
            request.PeriodCount,
            cancellationToken);

        var orderCountByDate = await _orderRepository.GetOrderCountByDateAsync(
            request.Period,
            request.PeriodCount,
            cancellationToken);

        var salesByCategory = await _orderRepository.GetSalesByCategoryAsync(cancellationToken);
        var ordersByStatus = await _orderRepository.GetOrdersByStatusAsync(cancellationToken);
        var topProducts = await _orderRepository.GetTopProductsAsync(10, cancellationToken);

        var chartData = new ChartDataDto
        {
            RevenueByDate = revenueByDate,
            OrderCountByDate = orderCountByDate,
            SalesByCategory = salesByCategory,
            OrdersByStatus = ordersByStatus,
            TopProducts = topProducts
        };

        return Result.Success(chartData);
    }
}







