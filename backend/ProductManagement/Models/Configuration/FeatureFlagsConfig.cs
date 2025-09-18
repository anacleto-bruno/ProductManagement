namespace ProductManagement.Models.Configuration;

public class FeatureFlagsConfig
{
    public bool EnableAdvancedLogging { get; set; }
    public bool UseRedisCache { get; set; }
    public bool EnableSwagger { get; set; }
}