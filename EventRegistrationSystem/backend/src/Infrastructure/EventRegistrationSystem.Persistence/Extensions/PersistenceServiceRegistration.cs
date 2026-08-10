namespace EventRegistrationSystem.Persistence.Extensions;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["DATABASE_CONNECTION_STRING"];

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        services.AddScoped<IPermissionService, EventRegistrationSystem.Persistence.Services.PermissionService>();


        return services;
    }
}
