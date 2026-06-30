
using System.Text.Json;

using StackExchange.Redis;

namespace TraineeApi.Services.Redis;

public class RedisService : IRedisService
{
    // private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _cache;
    private readonly ILogger<RedisService> _logger;
    public RedisService(IConnectionMultiplexer cache, ILogger<RedisService> logger)
    {
        _cache = cache;
        _logger = logger;
        // Console.WriteLine("Redis Object created");
    }

    public async Task<T?> GetAsync<T>(string key)
    {
            if (_cache.IsConnected)
            {
                var value = await _cache.GetDatabase().StringGetAsync(key);
            if( !value.HasValue)
            {
                return default;
            }
                return JsonSerializer.Deserialize<T>(value!);
            }
            return default;

    }

    public async Task RemoveAsync(string key)
    {
        if (_cache.IsConnected)
        {
            await _cache.GetDatabase().KeyDeleteAsync(key); 
        }
            // await _cache.RemoveAsync(key);
    }

    public async Task SetAsync<T>(string key, T value)
    {
        if (_cache.IsConnected)
        {
            string val = JsonSerializer.Serialize(value);
            await _cache.GetDatabase().StringSetAsync(
                key,
                val,
                TimeSpan.FromMinutes(1)
            );
        }
    }

}