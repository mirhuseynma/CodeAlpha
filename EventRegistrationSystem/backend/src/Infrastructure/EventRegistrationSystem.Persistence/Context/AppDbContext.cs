using EventRegistrationSystem.Application.Abstractions;
using EventRegistrationSystem.Domain.Entities;

namespace EventRegistrationSystem.Persistence.Context;

public class AppDbContext : IdentityDbContext<User, Role, Guid>, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // IdentityDbContext already exposes Users and Roles (as Users and Roles DB sets)
    // We explicitly implement the interface properties using the underlying DbSets.
    // However, IdentityDbContext exposes DbSet<User> Users { get; set; } and DbSet<Role> Roles { get; set; }
    // So we don't need to redeclare them, they satisfy the interface implicitly.

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Apply configurations from assembly if any
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
