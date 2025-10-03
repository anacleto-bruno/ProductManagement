using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductManagement.infrastructure;
using StackExchange.Redis;

namespace ProductManagement.services;

public interface IHealthCheckService
{
    Task<HealthCheckResult> CheckHealthAsync();
}

public record HealthCheckResult
{
    public string Status { get; init; } = string.Empty;
    public double DurationMs { get; init; }
    public Dictionary<string, ComponentHealth> Components { get; init; } = new();
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public record ComponentHealth
{
    public string Status { get; init; } = string.Empty;
    public string? Message { get; init; }
    public double DurationMs { get; init; }
    public Dictionary<string, object>? Data { get; init; }
}

public class HealthCheckService : IHealthCheckService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IConnectionMultiplexer? _redis;
    private readonly ILogger<HealthCheckService> _logger;

    public HealthCheckService(
        ApplicationDbContext dbContext,
        ILogger<HealthCheckService> logger,
        IConnectionMultiplexer? redis = null)
    {
        _dbContext = dbContext;
        _redis = redis;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var components = new Dictionary<string, ComponentHealth>();
        
        // Check Database Health
        var dbHealth = await CheckDatabaseHealthAsync();
        components.Add("database", dbHealth);

        // Check Redis Health (if configured)
        if (_redis != null)
        {
            var redisHealth = await CheckRedisHealthAsync();
            components.Add("redis", redisHealth);
        }

        // Check Application Health
        var appHealth = CheckApplicationHealth();
        components.Add("application", appHealth);

        stopwatch.Stop();

        // Determine overall status
        var overallStatus = components.Values.All(c => c.Status == "Healthy") 
            ? "Healthy" 
            : components.Values.Any(c => c.Status == "Unhealthy") 
                ? "Unhealthy" 
                : "Degraded";

        return new HealthCheckResult
        {
            Status = overallStatus,
            DurationMs = stopwatch.Elapsed.TotalMilliseconds,
            Components = components
        };
    }

    private async Task<ComponentHealth> CheckDatabaseHealthAsync()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            // Simple connectivity check
            var canConnect = await _dbContext.Database.CanConnectAsync();
            if (!canConnect)
            {
                return new ComponentHealth
                {
                    Status = "Unhealthy",
                    Message = "Cannot connect to database",
                    DurationMs = stopwatch.Elapsed.TotalMilliseconds
                };
            }

            // Check if we can query the database
            var productCount = await _dbContext.Products.CountAsync();
            var colorCount = await _dbContext.Colors.CountAsync();
            var sizeCount = await _dbContext.Sizes.CountAsync();

            stopwatch.Stop();

            return new ComponentHealth
            {
                Status = "Healthy",
                Message = "Database connection successful",
                DurationMs = stopwatch.Elapsed.TotalMilliseconds,
                Data = new Dictionary<string, object>
                {
                    ["products_count"] = productCount,
                    ["colors_count"] = colorCount,
                    ["sizes_count"] = sizeCount,
                    ["database_server"] = "Connected"
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            stopwatch.Stop();

            return new ComponentHealth
            {
                Status = "Unhealthy",
                Message = $"Database error: {ex.Message}",
                DurationMs = stopwatch.Elapsed.TotalMilliseconds
            };
        }
    }

    private async Task<ComponentHealth> CheckRedisHealthAsync()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            if (_redis == null || !_redis.IsConnected)
            {
                return new ComponentHealth
                {
                    Status = "Unhealthy",
                    Message = "Redis connection not available",
                    DurationMs = stopwatch.Elapsed.TotalMilliseconds
                };
            }

            var database = _redis.GetDatabase();
            
            // Test Redis with a simple ping
            var pingResult = await database.PingAsync();
            
            // Test set/get operation
            var testKey = "healthcheck:test";
            var testValue = DateTime.UtcNow.ToString();
            
            await database.StringSetAsync(testKey, testValue, TimeSpan.FromSeconds(30));
            var retrievedValue = await database.StringGetAsync(testKey);
            await database.KeyDeleteAsync(testKey);

            var isWorking = retrievedValue == testValue;

            stopwatch.Stop();

            return new ComponentHealth
            {
                Status = isWorking ? "Healthy" : "Degraded",
                Message = isWorking ? "Redis operations successful" : "Redis operations failed",
                DurationMs = stopwatch.Elapsed.TotalMilliseconds,
                Data = new Dictionary<string, object>
                {
                    ["ping_duration_ms"] = Math.Round(pingResult.TotalMilliseconds, 2),
                    ["connected_endpoints"] = _redis.GetEndPoints().Length,
                    ["is_connected"] = _redis.IsConnected
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Redis health check failed");
            stopwatch.Stop();

            return new ComponentHealth
            {
                Status = "Unhealthy",
                Message = $"Redis error: {ex.Message}",
                DurationMs = stopwatch.Elapsed.TotalMilliseconds
            };
        }
    }

    private ComponentHealth CheckApplicationHealth()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            // Check memory usage
            var workingSet = GC.GetTotalMemory(false);
            var gen0Collections = GC.CollectionCount(0);
            var gen1Collections = GC.CollectionCount(1);
            var gen2Collections = GC.CollectionCount(2);

            stopwatch.Stop();

            return new ComponentHealth
            {
                Status = "Healthy",
                Message = "Application running normally",
                DurationMs = stopwatch.Elapsed.TotalMilliseconds,
                Data = new Dictionary<string, object>
                {
                    ["memory_usage_mb"] = Math.Round(workingSet / 1024.0 / 1024.0, 2),
                    ["gc_gen0_collections"] = gen0Collections,
                    ["gc_gen1_collections"] = gen1Collections,
                    ["gc_gen2_collections"] = gen2Collections,
                    ["environment"] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                    ["dotnet_version"] = Environment.Version.ToString()
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Application health check failed");
            stopwatch.Stop();

            return new ComponentHealth
            {
                Status = "Unhealthy",
                Message = $"Application error: {ex.Message}",
                DurationMs = stopwatch.Elapsed.TotalMilliseconds
            };
        }
    }
}