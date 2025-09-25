using Microsoft.EntityFrameworkCore;
using ProductManagement.entities;

namespace ProductManagement.helpers;

public static class ProductQueryExtensions
{
    public static IQueryable<Product> IncludeRelated(this IQueryable<Product> query)
    {
        return query
            .Include(p => p.ProductColors)
                .ThenInclude(pc => pc.Color)
            .Include(p => p.ProductSizes)
                .ThenInclude(ps => ps.Size);
    }

    public static IQueryable<Product> WhereCategory(this IQueryable<Product> query, string? category)
    {
        return string.IsNullOrEmpty(category)
            ? query
            : query.Where(p => p.Category != null && p.Category.ToLower() == category.ToLower());
    }

    public static IQueryable<Product> WherePriceRange(this IQueryable<Product> query, decimal? minPrice, decimal? maxPrice)
    {
        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);
        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);
        return query;
    }

    public static IQueryable<Product> WhereSearch(this IQueryable<Product> query, string? searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
            return query;

        var lowerSearchTerm = searchTerm.ToLower();
        return query.Where(p =>
            p.Name.ToLower().Contains(lowerSearchTerm) ||
            (p.Description != null && p.Description.ToLower().Contains(lowerSearchTerm)) ||
            p.Brand.ToLower().Contains(lowerSearchTerm) ||
            p.Model.ToLower().Contains(lowerSearchTerm) ||
            p.Sku.ToLower().Contains(lowerSearchTerm));
    }

    public static IQueryable<Product> OrderByField(this IQueryable<Product> query, string? sortBy, bool descending = false)
    {
        return sortBy?.ToLower() switch
        {
            "name" => descending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            "price" => descending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
            "brand" => descending ? query.OrderByDescending(p => p.Brand) : query.OrderBy(p => p.Brand),
            "model" => descending ? query.OrderByDescending(p => p.Model) : query.OrderBy(p => p.Model),
            "category" => descending ? query.OrderByDescending(p => p.Category) : query.OrderBy(p => p.Category),
            "createdat" => descending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            _ => descending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt)
        };
    }
}