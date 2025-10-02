using Microsoft.Extensions.Logging;

namespace ProductManagement.services.caching;

/// <summary>
/// Fallback cache used when Redis is not configured/available. Performs no caching.
/// </summary>
public class NoOpCacheService : ICacheService
{
    private readonly ILogger<NoOpCacheService> _logger;
    public NoOpCacheService(ILogger<NoOpCacheService> logger) => _logger = logger;

    public Task<T?> GetAsync<T>(string key) => Task.FromResult(default(T));
    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) => Task.CompletedTask;
    public Task RemoveAsync(string key) => Task.CompletedTask;
    public Task RemoveAsync(IEnumerable<string> keys) => Task.CompletedTask;
    public Task AddToSetAsync(string setKey, string member) => Task.CompletedTask;
    public Task<string[]> GetSetMembersAsync(string setKey) => Task.FromResult(Array.Empty<string>());
}