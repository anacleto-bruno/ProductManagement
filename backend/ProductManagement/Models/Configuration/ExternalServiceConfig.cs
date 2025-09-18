namespace ProductManagement.Models.Configuration;

public class ExternalServiceConfig
{
    public string ApiBaseUrl { get; set; } = string.Empty;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}