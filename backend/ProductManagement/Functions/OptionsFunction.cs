using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductManagement.Infrastructure.Functions;
using System.Net;

namespace ProductManagement.Functions;

public class OptionsFunction : BaseFunction
{
    public OptionsFunction(ILogger<OptionsFunction> logger) : base(logger)
    {
    }

    [Function("OptionsHandler")]
    public HttpResponseData RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "options", Route = "{*route}")] HttpRequestData req)
    {
        _logger.LogInformation("Handling CORS preflight request");

        var response = req.CreateResponse(HttpStatusCode.OK);
        
        // Add CORS headers for preflight response
        response.Headers.Add("Access-Control-Allow-Origin", "*");
        response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization, Accept, Origin, X-Requested-With");
        response.Headers.Add("Access-Control-Max-Age", "86400");
        
        return response;
    }
}