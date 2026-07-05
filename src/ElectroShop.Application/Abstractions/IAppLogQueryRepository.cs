using ElectroShop.Domain.Entities;

namespace ElectroShop.Application.Abstractions;

public interface IAppLogQueryRepository
{
    Task<(IReadOnlyList<AppLogEntry> Items, int TotalCount)> GetLogsPagedAsync(
        int page,
        int pageSize,
        string? level = null,
        string? eventType = null,
        string? correlationId = null,
        Guid? userId = null,
        string? search = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        CancellationToken cancellationToken = default);
}
