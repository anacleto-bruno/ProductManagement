using ProductManagement.Dtos;

namespace ProductManagement.Services.Interfaces;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;
    Task RemoveAsync(string key);
    Task RemoveByPatternAsync(string pattern);
    Task<bool> ExistsAsync(string key);
    string GenerateProductSearchKey(ProductListRequestDto request);
    string GenerateProductDetailKey(int productId);
    Task InvalidateProductCacheAsync(int productId);
    Task InvalidateProductSearchCacheAsync();
    Task<CacheMetricsDto> GetCacheMetricsAsync();
}

public class CacheMetricsDto
{
    public long HitCount { get; set; }
    public long MissCount { get; set; }
    public double HitRatio { get; set; }
    public long TotalRequests { get; set; }
    public Dictionary<string, long> KeyPatternCounts { get; set; } = new();
}