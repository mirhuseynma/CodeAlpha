namespace LinkForge.Application.Common.Interfaces;

public interface IUrlVisitQueue
{
    ValueTask EnqueueAsync(UrlVisitEventDto visit, CancellationToken cancellationToken = default);
    IAsyncEnumerable<UrlVisitEventDto> DequeueAllAsync(CancellationToken cancellationToken = default);
}
