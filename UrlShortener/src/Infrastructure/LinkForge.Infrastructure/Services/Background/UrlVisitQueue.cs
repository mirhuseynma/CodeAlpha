namespace LinkForge.Infrastructure.Services.Background;

public class UrlVisitQueue : IUrlVisitQueue
{
    private readonly Channel<UrlVisit> _channel;

    public UrlVisitQueue()
    {
        var options = new BoundedChannelOptions(10_000)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _channel = Channel.CreateBounded<UrlVisit>(options);
    }

    public async ValueTask EnqueueAsync(UrlVisit visit, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(visit, cancellationToken);
    }

    public IAsyncEnumerable<UrlVisit> DequeueAllAsync(CancellationToken cancellationToken = default)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
