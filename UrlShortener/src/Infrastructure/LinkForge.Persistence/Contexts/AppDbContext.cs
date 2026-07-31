using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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
}

