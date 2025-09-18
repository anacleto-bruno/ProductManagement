using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductManagement.Infrastructure.Http;
using System.Net;

namespace ProductManagement.Infrastructure.Functions;

public abstract class BaseFunction
{
    protected readonly ILogger _logger;

    protected BaseFunction(ILogger logger)
    {
        _logger = logger;
    }

    protected async Task<HttpResponseData> ExecuteSafelyAsync<TResponse>(
        HttpRequestData request,
        Func<Task<TResponse>> handler)
    {
        try
        {
            var result = await handler();
            return await HttpResponseHelper.CreateJsonResponseAsync(request, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during request processing");
            return await HttpResponseHelper.CreateErrorResponseAsync(
                request, "An unexpected error occurred", HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}