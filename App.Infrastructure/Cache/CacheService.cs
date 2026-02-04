using System.Text.Json;
using App.Application.Abstractions.Cache;
using Microsoft.Extensions.Caching.Distributed;

namespace App.Infrastructure.Cache;

public sealed class CacheService(IDistributedCache distributedCache) : ICacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var cachedValue = await distributedCache.GetStringAsync(key, cancellationToken);
        
        return cachedValue is null ? default : JsonSerializer.Deserialize<T>(cachedValue);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10) // Default 10 dk
        };

        var jsonValue = JsonSerializer.Serialize(value);
        await distributedCache.SetStringAsync(key, jsonValue, options, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await distributedCache.RemoveAsync(key, cancellationToken);
    }
}