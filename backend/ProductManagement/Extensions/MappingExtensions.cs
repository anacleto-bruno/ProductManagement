using ProductManagement.Dtos;
using ProductManagement.Entities;

namespace ProductManagement.Extensions;

public static class MappingExtensions
{
    public static ProductResponseDto ToResponseDto(this Product product)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Brand = product.Brand,
            Sku = product.Sku,
            Price = product.Price,
            Category = product.Category,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    public static Product ToEntity(this CreateProductRequestDto dto)
    {
        return new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Brand = dto.Brand,
            Sku = dto.Sku,
            Price = dto.Price,
            Category = dto.Category,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateFromDto(this Product product, UpdateProductRequestDto dto)
    {
        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Brand = dto.Brand;
        product.Sku = dto.Sku;
        product.Price = dto.Price;
        product.Category = dto.Category;
        product.UpdatedAt = DateTime.UtcNow;
    }

    public static IEnumerable<ProductResponseDto> ToResponseDtos(this IEnumerable<Product> products)
    {
        return products.Select(p => p.ToResponseDto());
    }
}