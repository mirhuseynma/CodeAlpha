namespace LinkForge.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        var redisConnStr = configuration.GetConnectionString("Redis") ?? throw new InvalidOperationException("Redis ConnectionString not found.");
        
        services.AddSingleton<IConnectionMultiplexer>(sp => 
            ConnectionMultiplexer.Connect(redisConnStr));

        services.AddSingleton<ICacheService, RedisCacheService>();
        services.AddSingleton<IUrlShorteningService, Base62UrlShortenerService>();
        
        services.AddSingleton<IUrlVisitQueue, UrlVisitQueue>();
        services.AddHostedService<UrlVisitAnalyticsWorker>();
        services.AddHttpClient();

        return services;
    }
}
