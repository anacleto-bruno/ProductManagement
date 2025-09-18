using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text.Json;

namespace ProductManagement.Infrastructure.Http;

public static class HttpResponseHelper
{
    private static readonly JsonSerializerOptions DefaultJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static async Task<HttpResponseData> CreateJsonResponseAsync<T>(
        HttpRequestData request,
        T data,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var response = request.CreateResponse(statusCode);
        response.Headers.Add("Content-Type", "application/json");

        await response.WriteStringAsync(JsonSerializer.Serialize(data, DefaultJsonOptions));
        return response;
    }

    public static async Task<HttpResponseData> CreateErrorResponseAsync(
        HttpRequestData request,
        string message,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        object? details = null)
    {
        object errorObj = details != null
            ? new { Error = message, Details = details }
            : new { Error = message };

        return await CreateJsonResponseAsync(request, errorObj, statusCode);
    }

    public static async Task<HttpResponseData> CreateValidationErrorResponseAsync(
        HttpRequestData request,
        IEnumerable<ValidationError> errors)
    {
        var errorResponse = new
        {
            Error = "Validation failed",
            Details = errors.Select(e => new { Field = e.Field, Message = e.Message })
        };

        return await CreateJsonResponseAsync(request, errorResponse, HttpStatusCode.BadRequest);
    }
}

public record ValidationError(string Field, string Message);