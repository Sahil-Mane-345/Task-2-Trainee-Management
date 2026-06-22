
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace TraineeApi.Services.Redis;

public class RedisService : IRedisService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisService> _logger;
    public RedisService(IDistributedCache cache, ILogger<RedisService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var value = await _cache.GetStringAsync(key);
            if( value == null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(value)!;
        }
        catch (System.Exception)
        {
            _logger.LogError("Redis is not working");
            return default;
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _cache.RemoveAsync(key);
        }catch (System.Exception)
        {
            _logger.LogError("Redis is not working");
        }
    }

    public async Task SetAsync<T>(string key, T value)
    {
        try
        {
            string val =  JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, val, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            });
        }catch (System.Exception)
        {
            _logger.LogError("Redis is not working");
        }
    }

}