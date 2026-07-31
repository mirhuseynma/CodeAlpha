using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using LinkForge.Domain.Entities;
using LinkForge.Application.Common.Interfaces;
using System.Reflection;

namespace LinkForge.Persistence.Contexts;

public class AppDbContext : IdentityDbContext<AppUser, Role, Guid>, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ShortenedUrl> ShortenedUrls => Set<ShortenedUrl>();
    public DbSet<UrlVisit> UrlVisits => Set<UrlVisit>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var currentUserService = this.GetService<LinkForge.Application.Common.Interfaces.ICurrentUserService>();
        var timeProvider = this.GetService<TimeProvider>();
        
        var utcNow = timeProvider.GetUtcNow().UtcDateTime;
        var userId = currentUserService?.UserId ?? Guid.Empty; // Default system user

        foreach (var entry in ChangeTracker.Entries<LinkForge.Domain.Common.BaseAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = utcNow;
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.IsDeleted = false;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = utcNow;
                    entry.Entity.UpdatedBy = userId;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = utcNow;
                    entry.Entity.DeletedBy = userId;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
