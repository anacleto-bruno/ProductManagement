using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ProductManagement.helpers;
using ProductManagement.services;
using System.Net;

namespace ProductManagement.functions;

public class HealthCheckFunction
{
    private readonly ILogger<HealthCheckFunction> _logger;
    private readonly IHealthCheckService _healthCheckService;

    public HealthCheckFunction(
        ILogger<HealthCheckFunction> logger,
        IHealthCheckService healthCheckService)
    {
        _logger = logger;
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

        return await HttpResponseHelper.CreateJsonResponseAsync(req, healthResult, statusCode);
    }

    [Function("HealthCheckReady")]
    [OpenApiOperation(operationId: "HealthCheckReady", tags: new[] { "System" }, Summary = "Readiness probe", Description = "Returns OK if the service is ready to accept requests. Used by orchestrators like Kubernetes.")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "Service is ready")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.ServiceUnavailable, Description = "Service is not ready")]
    public async Task<HttpResponseData> GetReadinessAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health/ready")] HttpRequestData req)
    {
        var healthResult = await _healthCheckService.CheckHealthAsync();
        
        // For readiness, we only care about critical dependencies (database)
        var isDatabaseHealthy = healthResult.Components.ContainsKey("database") && 
                               healthResult.Components["database"].Status == "Healthy";

        var statusCode = isDatabaseHealthy ? HttpStatusCode.OK : HttpStatusCode.ServiceUnavailable;
        var status = isDatabaseHealthy ? "Ready" : "Not Ready";

        return await HttpResponseHelper.CreateJsonResponseAsync(req, 
            new { status, timestamp = DateTime.UtcNow }, statusCode);
    }

    [Function("HealthCheckLive")]
    [OpenApiOperation(operationId: "HealthCheckLive", tags: new[] { "System" }, Summary = "Liveness probe", Description = "Returns OK if the service is alive and running. Used by orchestrators like Kubernetes.")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "Service is alive")]
    public async Task<HttpResponseData> GetLivenessAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health/live")] HttpRequestData req)
    {
        // Simple liveness check - if we can respond, we're alive
        var uptimeMs = (DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime).TotalMilliseconds;
        
        return await HttpResponseHelper.CreateJsonResponseAsync(req, new 
        { 
            status = "Alive", 
            timestamp = DateTime.UtcNow,
            uptimeMs = uptimeMs
        });
    }
}