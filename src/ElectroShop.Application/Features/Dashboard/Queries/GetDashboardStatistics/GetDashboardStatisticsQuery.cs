using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Dashboard.Queries.GetDashboardStatistics;

/// <summary>
/// Admin dashboard satış və məhsul statistikalarını əldə etmək üçün Query
/// </summary>
public record GetDashboardStatisticsQuery : IRequest<Result<DashboardStatisticsResponse>>;
