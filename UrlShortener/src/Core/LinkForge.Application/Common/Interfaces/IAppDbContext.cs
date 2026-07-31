
namespace LinkForge.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<ShortenedUrl> ShortenedUrls { get; }
    DbSet<UrlVisit> UrlVisits { get; }
    DbSet<AppUser> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

