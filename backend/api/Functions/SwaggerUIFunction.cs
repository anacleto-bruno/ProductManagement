using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace ProductManagement.functions;

[ExcludeFromCodeCoverage]
public class SwaggerUIFunction
{
    private readonly ILogger<SwaggerUIFunction> _logger;

    public SwaggerUIFunction(ILogger<SwaggerUIFunction> logger)
    {
        _logger = logger;
    }

    [Function("SwaggerUI")]
    [OpenApiIgnore]
    public async Task<HttpResponseData> GetSwaggerUI(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "swagger")] HttpRequestData req)
    {
        _logger.LogInformation("Serving Swagger UI");
        
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/html");
        
        // Read the HTML file from wwwroot
        var htmlPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "swagger.html");
        
        if (File.Exists(htmlPath))
        {
            var html = await File.ReadAllTextAsync(htmlPath);
            await response.WriteStringAsync(html);
        }
        else
        {
            // Fallback inline HTML if file doesn't exist
            var html = """
                <!DOCTYPE html>
                <html lang="en">
                <head>
                    <meta charset="UTF-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                    <title>Product Management API - Swagger UI</title>
                    <link rel="stylesheet" type="text/css" href="https://unpkg.com/swagger-ui-dist@5.9.0/swagger-ui.css" />
                    <style>
                        html { box-sizing: border-box; overflow: -moz-scrollbars-vertical; overflow-y: scroll; }
                        *, *:before, *:after { box-sizing: inherit; }
                        body { margin:0; background: #fafafa; }
                    </style>
                </head>
                <body>
                    <div id="swagger-ui"></div>
                    <script src="https://unpkg.com/swagger-ui-dist@5.9.0/swagger-ui-bundle.js"></script>
                    <script src="https://unpkg.com/swagger-ui-dist@5.9.0/swagger-ui-standalone-preset.js"></script>
                    <script>
                        window.onload = function() {
                            const ui = SwaggerUIBundle({
                                url: '/api/openapi/v3.json',
                                dom_id: '#swagger-ui',
                                deepLinking: true,
                                presets: [
                                    SwaggerUIBundle.presets.apis,
                                    SwaggerUIStandalonePreset
                                ],
                                plugins: [
                                    SwaggerUIBundle.plugins.DownloadUrl
                                ],
                                layout: "StandaloneLayout"
                            });
                        };
                    </script>
                </body>
                </html>
                """;
            await response.WriteStringAsync(html);
        }
        
        return response;
    }
}