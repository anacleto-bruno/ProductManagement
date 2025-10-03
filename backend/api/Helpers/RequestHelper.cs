using Microsoft.Azure.Functions.Worker.Http;
using System.Text.Json;
using Microsoft.Extensions.Primitives;

namespace ProductManagement.helpers;

public static class RequestHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public static async Task<T> ParseJsonBodyAsync<T>(HttpRequestData request) where T : new()
    {
        try
        {
            var body = await new StreamReader(request.Body).ReadToEndAsync();
            
            if (string.IsNullOrEmpty(body))
                return new T();

            var result = JsonSerializer.Deserialize<T>(body, JsonOptions);
            return result ?? new T();
        }
        catch (JsonException)
        {
            throw new ArgumentException("Invalid JSON format");
        }
    }

    public static Dictionary<string, string> ParseQueryString(string queryString)
    {
        var result = new Dictionary<string, string>();
        if (string.IsNullOrEmpty(queryString))
            return result;

        // Remove the leading '?' if present
        if (queryString.StartsWith("?"))
            queryString = queryString.Substring(1);

        var pairs = queryString.Split('&');
        foreach (var pair in pairs)
        {
            var keyValue = pair.Split('=', 2);
            if (keyValue.Length == 2)
            {
                var key = Uri.UnescapeDataString(keyValue[0]);
                var value = Uri.UnescapeDataString(keyValue[1]);
                result[key] = value;
            }
        }

        return result;
    }
}