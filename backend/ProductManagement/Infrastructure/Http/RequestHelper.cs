using Microsoft.Azure.Functions.Worker.Http;
using System.Text.Json;

namespace ProductManagement.Infrastructure.Http;

public static class RequestHelper
{
    private static readonly JsonSerializerOptions DefaultJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<T> ParseJsonBodyAsync<T>(HttpRequestData request) where T : new()
    {
        if (request.Body.Length == 0)
        {
            return new T();
        }

        var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
        return JsonSerializer.Deserialize<T>(requestBody, DefaultJsonOptions) ?? new T();
    }

    public static async Task<T?> ParseJsonBodyOrNullAsync<T>(HttpRequestData request) where T : class
    {
        if (request.Body.Length == 0)
        {
            return null;
        }

        var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
        return JsonSerializer.Deserialize<T>(requestBody, DefaultJsonOptions);
    }
}