using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductManagement.Dtos;
using ProductManagement.Models.Configuration;
using ProductManagement.Services.Interfaces;
using StackExchange.Redis;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ProductManagement.Services;

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer? _redis;
    private readonly IDatabase? _database;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly CacheSettingsConfig _cacheSettings;
    private readonly FeatureFlagsConfig _featureFlags;

    private long _hitCount = 0;
    private long _missCount = 0;
    private readonly Dictionary<string, long> _keyPatternCounts = new();
    private readonly object _metricsLock = new();

    public RedisCacheService(
        IConnectionMultiplexer? redis,
        ILogger<RedisCacheService> logger,
        IOptions<CacheSettingsConfig> cacheSettings,
        IOptions<FeatureFlagsConfig> featureFlags)
    {
        _redis = redis;
        _database = redis?.GetDatabase();
        _logger = logger;
        _cacheSettings = cacheSettings.Value;
        _featureFlags = featureFlags.Value;

        if (_redis == null || !_redis.IsConnected)
        {
            _logger.LogWarning("Redis connection is not available. Cache operations will be bypassed.");
        }
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        if (!IsRedisAvailable())
        {
            LogCacheMiss(key, "Redis unavailable");
            return null;
        }

        try
        {
            var cacheKey = GenerateCacheKey(key);
            var cachedValue = await _database!.StringGetAsync(cacheKey);

            if (!cachedValue.HasValue)
            {
                LogCacheMiss(key, "Key not found");
                return null;
            }

            var deserializedValue = DeserializeValue<T>(cachedValue!);
            LogCacheHit(key);

            _logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
            return deserializedValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving value from cache for key: {Key}", key);
            LogCacheMiss(key, "Error");

            if (!_cacheSettings.CacheFailureFallbackEnabled)
                throw;

            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        if (!IsRedisAvailable() || value == null)
        {
            _logger.LogDebug("Skipping cache set for key: {Key} - Redis unavailable or value is null", key);
            return;
        }

        try
        {
            var cacheKey = GenerateCacheKey(key);
            var serializedValue = SerializeValue(value);
            var ttl = expiration ?? TimeSpan.FromMinutes(_cacheSettings.DefaultTtlMinutes);

            await _database!.StringSetAsync(cacheKey, serializedValue, ttl);

            _logger.LogDebug("Cached value for key: {CacheKey}, TTL: {TTL}", cacheKey, ttl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache value for key: {Key}", key);

            if (!_cacheSettings.CacheFailureFallbackEnabled)
                throw;
        }
    }

    public async Task RemoveAsync(string key)
    {
        if (!IsRedisAvailable())
        {
            _logger.LogDebug("Skipping cache remove for key: {Key} - Redis unavailable", key);
            return;
        }

        try
        {
            var cacheKey = GenerateCacheKey(key);
            await _database!.KeyDeleteAsync(cacheKey);

            _logger.LogDebug("Removed cache key: {CacheKey}", cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache key: {Key}", key);

            if (!_cacheSettings.CacheFailureFallbackEnabled)
                throw;
        }
    }

    public async Task RemoveByPatternAsync(string pattern)
    {
        if (!IsRedisAvailable())
        {
            _logger.LogDebug("Skipping cache pattern removal for pattern: {Pattern} - Redis unavailable", pattern);
            return;
        }

        try
        {
            var server = _redis!.GetServer(_redis.GetEndPoints().First());
            var cachePattern = GenerateCacheKey(pattern);
            var keys = server.Keys(pattern: cachePattern).ToArray();

            if (keys.Length > 0)
            {
                await _database!.KeyDeleteAsync(keys);
                _logger.LogInformation("Removed {Count} cache keys matching pattern: {Pattern}", keys.Length, cachePattern);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache keys by pattern: {Pattern}", pattern);

            if (!_cacheSettings.CacheFailureFallbackEnabled)
                throw;
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        if (!IsRedisAvailable())
        {
            return false;
        }

        try
        {
            var cacheKey = GenerateCacheKey(key);
            return await _database!.KeyExistsAsync(cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking cache key existence: {Key}", key);
            return false;
        }
    }

    public string GenerateProductSearchKey(ProductListRequestDto request)
    {
        var keyComponents = new List<string>
        {
            "products",
            "search",
            $"page:{request.Page}",
            $"perpage:{request.PerPage}",
            $"sort:{request.SortBy ?? "default"}",
            $"desc:{request.Descending}",
            $"search:{request.Search ?? ""}",
            $"category:{request.Category ?? ""}",
            $"brand:{request.Brand ?? ""}",
            $"minprice:{request.MinPrice?.ToString("F2") ?? ""}",
            $"maxprice:{request.MaxPrice?.ToString("F2") ?? ""}"
        };

        var key = string.Join(":", keyComponents);

        // Hash the key if it's too long
        if (key.Length > _cacheSettings.MaxCacheKeyLength)
        {
            key = $"products:search:hash:{GenerateHashKey(key)}";
        }

        return key;
    }

    public string GenerateProductDetailKey(int productId)
    {
        return $"products:detail:{productId}";
    }

    public async Task InvalidateProductCacheAsync(int productId)
    {
        try
        {
            // Remove specific product detail cache
            await RemoveAsync(GenerateProductDetailKey(productId));

            // Remove all product search caches since they might contain this product
            await RemoveByPatternAsync("products:search:*");

            _logger.LogInformation("Invalidated cache for product ID: {ProductId}", productId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating product cache for ID: {ProductId}", productId);
        }
    }

    public async Task InvalidateProductSearchCacheAsync()
    {
        try
        {
            await RemoveByPatternAsync("products:search:*");
            _logger.LogInformation("Invalidated all product search caches");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating product search cache");
        }
    }

    public Task<CacheMetricsDto> GetCacheMetricsAsync()
    {
        lock (_metricsLock)
        {
            var totalRequests = _hitCount + _missCount;
            var hitRatio = totalRequests > 0 ? (double)_hitCount / totalRequests : 0.0;

            return Task.FromResult(new CacheMetricsDto
            {
                HitCount = _hitCount,
                MissCount = _missCount,
                HitRatio = hitRatio,
                TotalRequests = totalRequests,
                KeyPatternCounts = new Dictionary<string, long>(_keyPatternCounts)
            });
        }
    }

    private bool IsRedisAvailable()
    {
        return _featureFlags.UseRedisCache && _redis != null && _redis.IsConnected && _database != null;
    }

    private string GenerateCacheKey(string key)
    {
        return $"{_cacheSettings.KeyPrefix}{key}";
    }

    private string GenerateHashKey(string input)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(hash)[0..16]; // Use first 16 characters of hash
    }

    private string SerializeValue<T>(T value) where T : class
    {
        var json = JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        if (_cacheSettings.EnableCacheCompression && json.Length > 1024)
        {
            return CompressString(json);
        }

        return json;
    }

    private T? DeserializeValue<T>(string value) where T : class
    {
        try
        {
            // Check if the value is compressed (simple heuristic - compressed strings start with specific bytes)
            var json = value.StartsWith("H4sI") || value.StartsWith("eJw") ? DecompressString(value) : value;

            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deserializing cached value");
            return null;
        }
    }

    private string CompressString(string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        using var memoryStream = new MemoryStream();
        using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
        {
            gzipStream.Write(bytes, 0, bytes.Length);
        }
        return Convert.ToBase64String(memoryStream.ToArray());
    }

    private string DecompressString(string compressedText)
    {
        var gzipBytes = Convert.FromBase64String(compressedText);
        using var memoryStream = new MemoryStream(gzipBytes);
        using var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
        using var reader = new StreamReader(gzipStream, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    private void LogCacheHit(string key)
    {
        if (!_cacheSettings.EnableCacheMetrics) return;

        lock (_metricsLock)
        {
            _hitCount++;
            var pattern = ExtractKeyPattern(key);
            _keyPatternCounts[pattern] = _keyPatternCounts.GetValueOrDefault(pattern, 0) + 1;
        }
    }

    private void LogCacheMiss(string key, string reason)
    {
        if (!_cacheSettings.EnableCacheMetrics) return;

        lock (_metricsLock)
        {
            _missCount++;
        }

        _logger.LogDebug("Cache miss for key: {Key}, Reason: {Reason}", key, reason);
    }

    private string ExtractKeyPattern(string key)
    {
        var parts = key.Split(':');
        return parts.Length >= 2 ? $"{parts[0]}:{parts[1]}" : parts[0];
    }
}