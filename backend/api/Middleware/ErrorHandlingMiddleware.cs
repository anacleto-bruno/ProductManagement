using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace ProductManagement.middleware;

public class ErrorHandlingMiddleware : IFunctionsWorkerMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred in function {FunctionName}", 
                context.FunctionDefinition.Name);

            // Get the HTTP context if available
            var httpRequestData = context.GetHttpRequestData();
            if (httpRequestData != null)
            {
                await HandleHttpException(context, ex, httpRequestData);
            }
        }
    }

    private async Task HandleHttpException(FunctionContext context, Exception exception, HttpRequestData req)
    {
        var (statusCode, message) = GetErrorResponse(exception);
        
        var response = req.CreateResponse(statusCode);
        response.Headers.Add("Content-Type", "application/json");

        var errorResponse = new
        {
            error = message,
            details = exception.Message,
            timestamp = DateTime.UtcNow
        };

        await response.WriteStringAsync(JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
        
        context.GetInvocationResult().Value = response;
    }

    private static (HttpStatusCode statusCode, string message) GetErrorResponse(Exception exception)
    {
        return exception switch
        {
            ArgumentException => (HttpStatusCode.BadRequest, "Invalid request parameters"),
            InvalidOperationException => (HttpStatusCode.Conflict, "Operation cannot be completed"),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Access denied"),
            NotImplementedException => (HttpStatusCode.NotImplemented, "Feature not implemented"),
            TimeoutException => (HttpStatusCode.RequestTimeout, "Request timeout"),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
        };
    }
}

public static class FunctionContextExtensions
{
    public static HttpRequestData? GetHttpRequestData(this FunctionContext context)
    {
        try
        {
            var keyValuePair = context.Features.SingleOrDefault(f => f.Key.Name == "IFunctionBindingsFeature");
            var bindingsFeature = keyValuePair.Value;
            var type = bindingsFeature?.GetType();
            var inputData = type?.GetProperties().SingleOrDefault(p => p.Name == "InputData")?.GetValue(bindingsFeature) as IReadOnlyDictionary<string, object>;
            return inputData?.Values.SingleOrDefault(o => o is HttpRequestData) as HttpRequestData;
        }
        catch
        {
            return null;
        }
    }
}