using ElectroShop.Application.Abstractions;
using ElectroShop.Domain.Entities;
using ElectroShop.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ElectroShop.Persistence.Repositories;

public class AppLogQueryRepository : IAppLogQueryRepository
{
    private readonly ElectroShopDbContext _dbContext;

    public AppLogQueryRepository(ElectroShopDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(IReadOnlyList<AppLogEntry> Items, int TotalCount)> GetLogsPagedAsync(
        int page,
        int pageSize,
        string? level = null,
        string? eventType = null,
        string? correlationId = null,
        Guid? userId = null,
        string? search = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.AppLogs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(level))
            query = query.Where(x => x.Level == level);

        if (!string.IsNullOrWhiteSpace(eventType))
            query = query.Where(x => x.EventType == eventType);

        if (!string.IsNullOrWhiteSpace(correlationId))
            query = query.Where(x => x.CorrelationId == correlationId);

        if (userId.HasValue)
            query = query.Where(x => x.UserId == userId);

        if (dateFrom.HasValue)
            query = query.Where(x => x.TimestampUtc >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(x => x.TimestampUtc <= dateTo.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x =>
                x.Message.Contains(term) ||
                (x.Exception != null && x.Exception.Contains(term)) ||
                (x.RequestPath != null && x.RequestPath.Contains(term)) ||
                (x.SourceContext != null && x.SourceContext.Contains(term)));
        }

        var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

        var items = await query
            .OrderByDescending(x => x.TimestampUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return (items, totalCount);
    }
}
