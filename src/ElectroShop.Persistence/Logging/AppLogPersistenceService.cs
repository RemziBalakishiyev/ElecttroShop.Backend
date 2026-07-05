using ElectroShop.Domain.Entities;
using ElectroShop.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ElectroShop.Persistence.Logging;

public sealed class AppLogPersistenceService : BackgroundService
{
    private const int BatchSize = 50;
    private static readonly TimeSpan FlushInterval = TimeSpan.FromSeconds(2);

    private readonly AppLogWriter _writer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AppLogPersistenceService> _logger;

    public AppLogPersistenceService(
        AppLogWriter writer,
        IServiceScopeFactory scopeFactory,
        ILogger<AppLogPersistenceService> logger)
    {
        _writer = writer;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var batch = new List<AppLogEntry>(BatchSize);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                timeoutCts.CancelAfter(FlushInterval);

                while (batch.Count < BatchSize)
                {
                    if (await _writer.Reader.WaitToReadAsync(timeoutCts.Token).ConfigureAwait(false))
                    {
                        while (batch.Count < BatchSize && _writer.Reader.TryRead(out var entry))
                        {
                            batch.Add(entry);
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (batch.Count > 0)
                {
                    await PersistBatchAsync(batch, stoppingToken).ConfigureAwait(false);
                    batch.Clear();
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist application log batch");
                batch.Clear();
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken).ConfigureAwait(false);
            }
        }

        while (_writer.Reader.TryRead(out var remaining))
        {
            batch.Add(remaining);
        }

        if (batch.Count > 0)
        {
            await PersistBatchAsync(batch, CancellationToken.None).ConfigureAwait(false);
        }
    }

    private async Task PersistBatchAsync(IReadOnlyList<AppLogEntry> batch, CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ElectroShopDbContext>();

        await dbContext.AppLogs.AddRangeAsync(batch, cancellationToken).ConfigureAwait(false);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
