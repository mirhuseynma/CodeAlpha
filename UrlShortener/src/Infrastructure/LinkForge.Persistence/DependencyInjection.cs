namespace LinkForge.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IIdentityService, LinkForge.Persistence.Identity.IdentityService>();
        services.AddScoped<IPermissionService, LinkForge.Persistence.Identity.PermissionService>();

        return services;
    }
}
