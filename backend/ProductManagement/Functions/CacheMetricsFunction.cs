using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductManagement.Infrastructure.Functions;
using ProductManagement.Services.Interfaces;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;

namespace ProductManagement.Functions;

public class CacheMetricsFunction : BaseFunction
{
    private readonly ICacheService _cacheService;

    public CacheMetricsFunction(
        ILogger<CacheMetricsFunction> logger,
        ICacheService cacheService)
        : base(logger)
    {
        _cacheService = cacheService;
    }

    [Function("GetCacheMetrics")]
    [OpenApiOperation(operationId: "GetCacheMetrics", tags: new[] { "Monitoring" }, Summary = "Get cache performance metrics", Description = "Retrieves cache hit/miss statistics and performance metrics")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(object), Summary = "Cache metrics retrieved successfully", Description = "Returns cache performance statistics")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Summary = "Internal server error", Description = "An error occurred while retrieving cache metrics")]
    public async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "cache/metrics")] HttpRequestData req)
    {
        _logger.LogInformation("Getting cache metrics");

        return await ExecuteSafelyAsync<object>(req, async () =>
        {
            var metrics = await _cacheService.GetCacheMetricsAsync();

            _logger.LogInformation("Cache metrics retrieved - Hit ratio: {HitRatio:P2}, Total requests: {TotalRequests}",
                metrics.HitRatio, metrics.TotalRequests);

            return new
            {
                metrics.HitCount,
                metrics.MissCount,
                metrics.HitRatio,
                metrics.TotalRequests,
                HitRatioPercentage = $"{metrics.HitRatio:P2}",
                metrics.KeyPatternCounts,
                Timestamp = DateTime.UtcNow
            };
        });
    }
}