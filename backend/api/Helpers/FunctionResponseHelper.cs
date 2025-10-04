using Microsoft.Azure.Functions.Worker.Http;
using ProductManagement.models;
using System.Net;
using System.Text.Json;

namespace ProductManagement.helpers;

public static class FunctionResponseHelper
{
    /// <summary>
    /// Creates a standardized HTTP response from a service result
    /// </summary>
    public static async Task<HttpResponseData> CreateResponseAsync<T>(
        HttpRequestData request, 
        Result<T> result, 
        HttpStatusCode successStatusCode = HttpStatusCode.OK)
    {
        if (result.IsSuccess)
        {
            var response = request.CreateResponse(successStatusCode);
            await WriteJsonAsync(response, result.Data);
            return response;
        }

        // Determine appropriate error status code
        var statusCode = DetermineErrorStatusCode(result.ErrorMessage);
    var errorResponse = request.CreateResponse(statusCode);
    await WriteJsonAsync(errorResponse, new { error = result.ErrorMessage });
    return errorResponse;
    }

    /// <summary>
    /// Creates a standardized HTTP response from a service result without data
    /// </summary>
    public static async Task<HttpResponseData> CreateResponseAsync(
        HttpRequestData request, 
        Result result, 
        HttpStatusCode successStatusCode = HttpStatusCode.OK)
    {
        if (result.IsSuccess)
        {
            var response = request.CreateResponse(successStatusCode);
            await WriteJsonAsync(response, new { message = "Operation completed successfully" });
            return response;
        }

        // Determine appropriate error status code
        var statusCode = DetermineErrorStatusCode(result.ErrorMessage);
    var errorResponse = request.CreateResponse(statusCode);
    await WriteJsonAsync(errorResponse, new { error = result.ErrorMessage });
    return errorResponse;
    }

    /// <summary>
    /// Creates a bad request response for invalid input parameters
    /// </summary>
    public static async Task<HttpResponseData> CreateBadRequestAsync(
        HttpRequestData request, 
        string errorMessage)
    {
        var response = request.CreateResponse(HttpStatusCode.BadRequest);
        await WriteJsonAsync(response, new { error = errorMessage });
        return response;
    }

    /// <summary>
    /// Creates a custom response with the specified status code and data
    /// </summary>
    public static async Task<HttpResponseData> CreateCustomResponseAsync<T>(
        HttpRequestData request,
        HttpStatusCode statusCode,
        T data)
    {
        var response = request.CreateResponse(statusCode);
        await WriteJsonAsync(response, data);
        return response;
    }

    private static HttpStatusCode DetermineErrorStatusCode(string? errorMessage)
    {
        if (string.IsNullOrEmpty(errorMessage))
            return HttpStatusCode.BadRequest;

        if (errorMessage.Contains("not found", StringComparison.OrdinalIgnoreCase))
            return HttpStatusCode.NotFound;

        if (errorMessage.Contains("validation", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("invalid", StringComparison.OrdinalIgnoreCase))
            return HttpStatusCode.BadRequest;

        return HttpStatusCode.BadRequest;
    }

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private static async Task WriteJsonAsync(HttpResponseData response, object? value)
    {
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        if (value is null)
        {
            await response.Body.WriteAsync("null"u8.ToArray());
            return;
        }
        var json = JsonSerializer.Serialize(value, _jsonOptions);
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        await response.Body.WriteAsync(bytes, 0, bytes.Length);
        response.Body.Seek(0, SeekOrigin.Begin);
    }
}