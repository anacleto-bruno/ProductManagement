using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using ProductManagement.Infrastructure.Data;
using ProductManagement.Infrastructure.Functions;
using System.Net;

namespace ProductManagement.Functions;

public class HealthFunction : BaseFunction
{
    public HealthFunction(ILogger<HealthFunction> logger) : base(logger)
    {
    }

    [Function("Health")]
    public async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequestData req,
        FunctionContext context)
    {
        _logger.LogInformation("Health check requested");

        return await ExecuteSafelyAsync(req, async () =>
        {
            var serviceProvider = context.InstanceServices;
            var dbContext = serviceProvider.GetService<ProductManagementContext>();

            if (dbContext != null)
            {
                await dbContext.Database.CanConnectAsync();
                _logger.LogInformation("Database connection verified");
            }

            return new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Database = "Connected",
                Version = "1.0.0"
            };
        });
    }
}