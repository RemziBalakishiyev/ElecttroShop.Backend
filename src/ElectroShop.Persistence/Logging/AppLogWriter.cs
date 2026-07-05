using System.Threading.Channels;
using ElectroShop.Domain.Entities;

namespace ElectroShop.Persistence.Logging;

public interface IAppLogWriter
{
    ValueTask EnqueueAsync(AppLogEntry entry, CancellationToken cancellationToken = default);
}

public sealed class AppLogWriter : IAppLogWriter
{
    private readonly Channel<AppLogEntry> _channel;

    public AppLogWriter()
    {
        _channel = Channel.CreateBounded<AppLogEntry>(new BoundedChannelOptions(10_000)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = true,
            SingleWriter = false
        });
    }

    internal ChannelReader<AppLogEntry> Reader => _channel.Reader;

    public ValueTask EnqueueAsync(AppLogEntry entry, CancellationToken cancellationToken = default)
        => _channel.Writer.WriteAsync(entry, cancellationToken);
}
