namespace ProductManagement.Models.Configuration;

public class CacheSettingsConfig
{
    public int DefaultTtlMinutes { get; set; } = 5;
    public int ProductSearchTtlMinutes { get; set; } = 5;
    public int ProductDetailTtlMinutes { get; set; } = 10;
    public bool EnableCacheCompression { get; set; } = true;
    public string KeyPrefix { get; set; } = "pm:";
    public int MaxCacheKeyLength { get; set; } = 250;
    public bool EnableCacheMetrics { get; set; } = true;
    public bool CacheFailureFallbackEnabled { get; set; } = true;
}