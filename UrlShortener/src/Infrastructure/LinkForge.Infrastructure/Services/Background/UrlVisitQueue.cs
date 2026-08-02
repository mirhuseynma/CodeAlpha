namespace LinkForge.Infrastructure.Services.Background;

public class UrlVisitQueue : IUrlVisitQueue
{
    private readonly Channel<UrlVisitEventDto> _channel;

    public UrlVisitQueue()
    {
        var options = new BoundedChannelOptions(10_000)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _channel = Channel.CreateBounded<UrlVisitEventDto>(options);
    }

    public async ValueTask EnqueueAsync(UrlVisitEventDto visit, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(visit, cancellationToken);
    }

    public IAsyncEnumerable<UrlVisitEventDto> DequeueAllAsync(CancellationToken cancellationToken = default)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
