using LinkForge.Domain.Entities;

namespace LinkForge.Application.Common.Interfaces;

public interface IUrlVisitQueue
{
    ValueTask EnqueueAsync(UrlVisit visit, CancellationToken cancellationToken = default);
    IAsyncEnumerable<UrlVisit> DequeueAllAsync(CancellationToken cancellationToken = default);
}
