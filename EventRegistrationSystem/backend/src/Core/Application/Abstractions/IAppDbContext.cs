using EventRegistrationSystem.Domain.Entities;

namespace EventRegistrationSystem.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
