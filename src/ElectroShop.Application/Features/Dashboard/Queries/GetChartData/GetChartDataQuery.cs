using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Dashboard.Queries.GetChartData;

/// <summary>
/// Chart məlumatlarını əldə etmək üçün Query
/// </summary>
public record GetChartDataQuery : IRequest<Result<ChartDataDto>>
{
    /// <summary>
    /// Zaman intervalı tipi: "daily", "weekly", "monthly"
    /// </summary>
    public string Period { get; init; } = "monthly";

    /// <summary>
    /// Neçə period geriyə getmək (məs: 12 ay, 30 gün)
    /// </summary>
    public int PeriodCount { get; init; } = 12;
}





