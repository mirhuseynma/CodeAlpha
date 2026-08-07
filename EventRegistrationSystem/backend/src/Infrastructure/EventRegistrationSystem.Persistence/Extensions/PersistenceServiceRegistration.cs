using EventRegistrationSystem.Application.Abstractions;
using EventRegistrationSystem.Domain.Entities;
using EventRegistrationSystem.Persistence.Context;
using Microsoft.AspNetCore.Identity;

namespace EventRegistrationSystem.Persistence.Extensions;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["DATABASE_CONNECTION_STRING"];

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        // Setup Identity Core
        services.AddIdentityCore<User>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        })
        .AddRoles<Role>()
        .AddEntityFrameworkStores<AppDbContext>();

        return services;
    }
}
