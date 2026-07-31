using System.Text.Json;
using StackExchange.Redis;

namespace LinkForge.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _cacheDb;

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer) => _cacheDb = connectionMultiplexer.GetDatabase();

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var cachedValue = await _cacheDb.StringGetAsync(key);
        if (!cachedValue.HasValue) return default;

        return JsonSerializer.Deserialize<T>((string)cachedValue!);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default)
    {
        var serializedValue = JsonSerializer.Serialize(value);
        await _cacheDb.StringSetAsync(key, serializedValue, expirationTime ?? TimeSpan.FromMinutes(60));
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cacheDb.KeyDeleteAsync(key);
    }
}
