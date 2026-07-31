using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using LinkForge.Infrastructure.Caching;
using LinkForge.Infrastructure.Services;
using LinkForge.Infrastructure.Services.Background;

namespace LinkForge.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnStr = configuration.GetConnectionString("Redis") ?? throw new InvalidOperationException("Redis ConnectionString not found.");
        
        services.AddSingleton<IConnectionMultiplexer>(sp => 
            ConnectionMultiplexer.Connect(redisConnStr));

        services.AddSingleton<ICacheService, RedisCacheService>();
        services.AddSingleton<IUrlShorteningService, Base62UrlShortenerService>();
        
        services.AddSingleton<IUrlVisitQueue, UrlVisitQueue>();
        services.AddHostedService<UrlVisitAnalyticsWorker>();
        services.AddHostedService<UrlVisitRecoveryHostedService>();

        return services;
    }
}
