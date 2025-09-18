namespace ProductManagement.Models.Configuration;

public class ApplicationSettingsConfig
{
    public int MaxPageSize { get; set; } = 100;
    public int DefaultPageSize { get; set; } = 20;
    public int CacheExpirationMinutes { get; set; } = 5;
    public int MaxRetryCount { get; set; } = 3;
}