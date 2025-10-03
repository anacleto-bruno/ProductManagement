using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ProductManagement.functions.@base;
using ProductManagement.services;
using System.Net;

namespace ProductManagement.functions;

public class HealthCheckFunction : BaseFunction
{
    private readonly IHealthCheckService _healthCheckService;

    public HealthCheckFunction(
        ILogger<HealthCheckFunction> logger,
        IHealthCheckService healthCheckService) 
        : base(logger)
    {
        _healthCheckService = healthCheckService;
    }

    [Function("HealthCheck")]
    [OpenApiOperation(operationId: "HealthCheck", tags: new[] { "System" }, Summary = "System health check", Description = "Returns the current health status of the API and its dependencies including database and cache connectivity.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(HealthCheckResult), Description = "System is healthy")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.ServiceUnavailable, contentType: "application/json", bodyType: typeof(HealthCheckResult), Description = "System is unhealthy")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.PartialContent, contentType: "application/json", bodyType: typeof(HealthCheckResult), Description = "System is degraded")]
    public async Task<HttpResponseData> GetHealthAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequestData req)
    {
        return await ExecuteSafelyAsync(req, async () =>
        {
            _logger.LogInformation("Health check requested");
            
            var healthResult = await _healthCheckService.CheckHealthAsync();
            
            // Log health check results
            _logger.LogInformation("Health check completed: {Status} in {Duration}ms", 
                healthResult.Status, healthResult.DurationMs);

            // Return appropriate HTTP status based on health
            var statusCode = healthResult.Status switch
            {
                "Healthy" => HttpStatusCode.OK,
                "Degraded" => HttpStatusCode.PartialContent,
                _ => HttpStatusCode.ServiceUnavailable
            };

            var response = req.CreateResponse(statusCode);
            response.Headers.Add("Content-Type", "application/json");
            
            // Manual JSON serialization to avoid complex object serialization issues
            var json = System.Text.Json.JsonSerializer.Serialize(healthResult);
            await response.WriteStringAsync(json);
            
            return response;
        });
    }

    [Function("HealthCheckReady")]
    [OpenApiOperation(operationId: "HealthCheckReady", tags: new[] { "System" }, Summary = "Readiness probe", Description = "Returns OK if the service is ready to accept requests. Used by orchestrators like Kubernetes.")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "Service is ready")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.ServiceUnavailable, Description = "Service is not ready")]
    public async Task<HttpResponseData> GetReadinessAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health/ready")] HttpRequestData req)
    {
        return await ExecuteSafelyAsync(req, async () =>
        {
            var healthResult = await _healthCheckService.CheckHealthAsync();
            
            // For readiness, we only care about critical dependencies (database)
            var isDatabaseHealthy = healthResult.Components.ContainsKey("database") && 
                                   healthResult.Components["database"].Status == "Healthy";

            var statusCode = isDatabaseHealthy ? HttpStatusCode.OK : HttpStatusCode.ServiceUnavailable;
            var status = isDatabaseHealthy ? "Ready" : "Not Ready";

            var response = req.CreateResponse(statusCode);
            response.Headers.Add("Content-Type", "application/json");
            
            var json = System.Text.Json.JsonSerializer.Serialize(new { status, timestamp = DateTime.UtcNow });
            await response.WriteStringAsync(json);
            
            return response;
        });
    }

    [Function("HealthCheckLive")]
    [OpenApiOperation(operationId: "HealthCheckLive", tags: new[] { "System" }, Summary = "Liveness probe", Description = "Returns OK if the service is alive and running. Used by orchestrators like Kubernetes.")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "Service is alive")]
    public async Task<HttpResponseData> GetLivenessAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health/live")] HttpRequestData req)
    {
        return await ExecuteSafelyAsync(req, async () =>
        {
            // Simple liveness check - if we can respond, we're alive
            var uptimeMs = (DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime).TotalMilliseconds;
            
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            
            var json = System.Text.Json.JsonSerializer.Serialize(new 
            { 
                status = "Alive", 
                timestamp = DateTime.UtcNow,
                uptimeMs = uptimeMs
            });
            await response.WriteStringAsync(json);
            
            return response;
        });
    }
}