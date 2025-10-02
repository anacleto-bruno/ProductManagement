using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace ProductManagement.services.caching;

/// <summary>
/// Redis-backed cache implementation. Uses basic StringGet/Set and Set data structures.
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly IDatabase _db;

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisCacheService> logger)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _logger = logger;
        _db = _connectionMultiplexer.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var value = await _db.StringGetAsync(key);
            if (!value.HasValue) return default;
            return CacheSerializer.Deserialize<T>(value!);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache get failed for key {CacheKey}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        try
        {
            var json = CacheSerializer.Serialize(value);
            await _db.StringSetAsync(key, json, expiry);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache set failed for key {CacheKey}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _db.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache remove failed for key {CacheKey}", key);
        }
    }

    public async Task RemoveAsync(IEnumerable<string> keys)
    {
        try
        {
            var redisKeys = keys.Select(k => (RedisKey)k).ToArray();
            if (redisKeys.Length > 0)
            {
                await _db.KeyDeleteAsync(redisKeys);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache bulk remove failed");
        }
    }

    public async Task AddToSetAsync(string setKey, string member)
    {
        try
        {
            await _db.SetAddAsync(setKey, member);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache set add failed for key {SetKey}", setKey);
        }
    }

    public async Task<string[]> GetSetMembersAsync(string setKey)
    {
        try
        {
            var members = await _db.SetMembersAsync(setKey);
            return members.Select(m => m.ToString()).ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache set members retrieval failed for key {SetKey}", setKey);
            return Array.Empty<string>();
        }
    }
}