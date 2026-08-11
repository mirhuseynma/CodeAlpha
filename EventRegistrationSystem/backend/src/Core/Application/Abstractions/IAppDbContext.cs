namespace EventRegistrationSystem.Application.Abstractions;

public interface IAppDbContext
{
    Microsoft.EntityFrameworkCore.DbSet<User> Users { get; }
    Microsoft.EntityFrameworkCore.DbSet<Role> Roles { get; }
    Microsoft.EntityFrameworkCore.DbSet<EventRegistrationSystem.Domain.Entities.Event> Events { get; }
    Microsoft.EntityFrameworkCore.DbSet<EventRegistrationSystem.Domain.Entities.Registration> Registrations { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

