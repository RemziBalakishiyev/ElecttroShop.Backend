using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.AppLogs.Queries.GetAppLogs;

public class GetAppLogsQuery : IRequest<PagedResult<AppLogDto>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Level { get; init; }
    public string? EventType { get; init; }
    public string? CorrelationId { get; init; }
    public Guid? UserId { get; init; }
    public string? Search { get; init; }
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
}
