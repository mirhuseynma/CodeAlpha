
namespace EventRegistrationSystem.Persistence.Context;

public class AppDbContext : IdentityDbContext<User, Role, Guid>, IAppDbContext
{
    public DbSet<Event> Events { get; set; }
    public DbSet<Registration> Registrations { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Event>()
            .HasOne(e => e.Organizer)
            .WithMany()
            .HasForeignKey(e => e.OrganizerId)
            .IsRequired();
            
        builder.Entity<Registration>()
            .HasOne(r => r.Event)
            .WithMany()
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.Entity<Registration>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Registration>()
            .HasIndex(r => new { r.EventId, r.UserId })
            .IsUnique();
        
        // Apply configurations from assembly if any
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}

