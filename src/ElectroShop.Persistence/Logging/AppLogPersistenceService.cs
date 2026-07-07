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
                DrainAvailableEntries(batch);

                if (batch.Count >= BatchSize)
                {
                    await PersistBatchAsync(batch, stoppingToken).ConfigureAwait(false);
                    batch.Clear();
                    continue;
                }

                var readTask = _writer.Reader.WaitToReadAsync(stoppingToken).AsTask();
                var delayTask = Task.Delay(FlushInterval, stoppingToken);

                var completed = await Task.WhenAny(readTask, delayTask).ConfigureAwait(false);

                if (completed == readTask)
                {
                    _ = await readTask.ConfigureAwait(false);
                    DrainAvailableEntries(batch);
                }

                if (batch.Count > 0 && (batch.Count >= BatchSize || completed == delayTask))
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
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        DrainAvailableEntries(batch);

        if (batch.Count > 0)
        {
            await PersistBatchAsync(batch, CancellationToken.None).ConfigureAwait(false);
        }
    }

    private void DrainAvailableEntries(List<AppLogEntry> batch)
    {
        while (batch.Count < BatchSize && _writer.Reader.TryRead(out var entry))
        {
            batch.Add(entry);
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
