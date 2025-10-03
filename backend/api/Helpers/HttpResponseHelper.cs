using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace ProductManagement.helpers;

public static class HttpResponseHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public static async Task<HttpResponseData> CreateJsonResponseAsync<T>(
        HttpRequestData request, T data, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var response = request.CreateResponse(statusCode);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        
        var json = JsonSerializer.Serialize(data, JsonOptions);
        await response.WriteStringAsync(json);
        
        return response;
    }

    public static async Task<HttpResponseData> CreateErrorResponseAsync(
        HttpRequestData request, string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        var errorResponse = new { error = message };
        return await CreateJsonResponseAsync(request, errorResponse, statusCode);
    }

    public static async Task<HttpResponseData> CreateValidationErrorResponseAsync(
        HttpRequestData request, List<string> errors)
    {
        var errorResponse = new { errors = errors };
        return await CreateJsonResponseAsync(request, errorResponse, HttpStatusCode.UnprocessableEntity);
    }
}