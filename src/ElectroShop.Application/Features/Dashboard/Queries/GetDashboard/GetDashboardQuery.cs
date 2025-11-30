using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Dashboard.Queries.GetDashboard;

/// <summary>
/// Dashboard məlumatlarını əldə etmək üçün Query
/// </summary>
public record GetDashboardQuery : IRequest<Result<DashboardDto>>;




