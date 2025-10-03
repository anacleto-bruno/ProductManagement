namespace ProductManagement.services.caching;

/// <summary>
/// Configuration options for caching behavior.
/// </summary>
public class CacheOptions
{
    public const string SectionName = "Cache";

    /// <summary>
    /// Default time-to-live for all cached items. Default is 5 minutes.
    /// </summary>
    public TimeSpan DefaultTtl { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Whether caching is enabled. Default is true.
    /// </summary>
    public bool Enabled { get; set; } = true;
}