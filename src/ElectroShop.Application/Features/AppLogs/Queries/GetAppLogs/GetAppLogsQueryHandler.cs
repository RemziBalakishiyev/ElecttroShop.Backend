using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.AppLogs.Queries.GetAppLogs;

public class GetAppLogsQueryHandler : IRequestHandler<GetAppLogsQuery, PagedResult<AppLogDto>>
{
    private readonly IAppLogQueryRepository _appLogQueryRepository;

    public GetAppLogsQueryHandler(IAppLogQueryRepository appLogQueryRepository)
    {
        _appLogQueryRepository = appLogQueryRepository;
    }

    public async Task<PagedResult<AppLogDto>> Handle(GetAppLogsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _appLogQueryRepository.GetLogsPagedAsync(
            request.Page,
            request.PageSize,
            request.Level,
            request.EventType,
            request.CorrelationId,
            request.UserId,
            request.Search,
            request.DateFrom,
            request.DateTo,
            cancellationToken);

        if (totalCount == 0)
            return PagedResult<AppLogDto>.Empty(request.Page, request.PageSize);

        var dtos = items.Select(log => new AppLogDto
        {
            Id = log.Id,
            TimestampUtc = log.TimestampUtc,
            Level = log.Level,
            Message = log.Message,
            Exception = log.Exception,
            SourceContext = log.SourceContext,
            EventType = log.EventType,
            CorrelationId = log.CorrelationId,
            UserId = log.UserId,
            UserEmail = log.UserEmail,
            RequestPath = log.RequestPath,
            RequestMethod = log.RequestMethod,
            QueryString = log.QueryString,
            RequestBody = log.RequestBody,
            ResponseStatusCode = log.ResponseStatusCode,
            ElapsedMilliseconds = log.ElapsedMilliseconds,
            ClientIp = log.ClientIp,
            UserAgent = log.UserAgent,
            MachineName = log.MachineName,
            PropertiesJson = log.PropertiesJson
        }).ToList();

        return PagedResult<AppLogDto>.Success(dtos, request.Page, request.PageSize, totalCount);
    }
}
