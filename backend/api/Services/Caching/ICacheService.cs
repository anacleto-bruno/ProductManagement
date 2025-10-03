using System.Text.Json;

namespace ProductManagement.services.caching;

/// <summary>
/// Abstraction over a distributed cache (Redis) with minimal operations needed by the domain.
/// Provides simple string key/value semantics with JSON serialization plus basic set tracking
/// used for group invalidation (e.g., paged product queries).
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task RemoveAsync(string key);
    Task RemoveAsync(IEnumerable<string> keys);

    // Set tracking utilities (used to track paged product query keys for invalidation)
    Task AddToSetAsync(string setKey, string member);
    Task<string[]> GetSetMembersAsync(string setKey);
    Task RemoveKeyAsync(string key) => RemoveAsync(key);
}

internal static class CacheSerializer
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public static string Serialize<T>(T value) => JsonSerializer.Serialize(value, Options);
    public static T? Deserialize<T>(string? json) => json is null ? default : JsonSerializer.Deserialize<T>(json, Options);
}