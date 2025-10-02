using System.Security.Cryptography;
using System.Text;
using ProductManagement.dtos;

namespace ProductManagement.services.caching;

public static class ProductCacheKeys
{
    public const string PagedIndexSet = "product:paged:index";

    public static string ById(int id) => $"product:by-id:{id}";

    public static string Paged(PaginationRequestDto request)
    {
        // Create a canonical signature of the request
        var signature = $"p={request.Page}|s={request.PageSize}|search={request.SearchTerm}|cat={request.Category}|min={request.MinPrice}|max={request.MaxPrice}|sort={request.SortBy}|desc={request.Descending}";
        var hash = ComputeSha256(signature);
        return $"product:paged:{hash}";
    }

    private static string ComputeSha256(string raw)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
        var sb = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }
}