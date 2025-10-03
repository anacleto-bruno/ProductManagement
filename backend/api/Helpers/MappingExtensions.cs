using ProductManagement.dtos;
using ProductManagement.entities;

namespace ProductManagement.helpers;

public static class MappingExtensions
{
    public static ProductResponseDto ToResponseDto(this Product product)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Model = product.Model,
            Brand = product.Brand,
            Sku = product.Sku,
            Price = product.Price,
            Category = product.Category,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            Colors = product.ProductColors.Select(pc => pc.Color.ToDto()).ToList(),
            Sizes = product.ProductSizes.Select(ps => ps.Size.ToDto()).ToList()
        };
    }

    public static Product ToEntity(this CreateProductRequestDto dto)
    {
        return new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Model = dto.Model,
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
        product.Model = dto.Model;
        product.Brand = dto.Brand;
        product.Sku = dto.Sku;
        product.Price = dto.Price;
        product.Category = dto.Category;
        product.UpdatedAt = DateTime.UtcNow;
    }

    public static ColorDto ToDto(this Color color)
    {
        return new ColorDto
        {
            Id = color.Id,
            Name = color.Name,
            HexCode = color.HexCode
        };
    }

    public static SizeDto ToDto(this Size size)
    {
        return new SizeDto
        {
            Id = size.Id,
            Name = size.Name,
            Code = size.Code,
            SortOrder = size.SortOrder
        };
    }

    public static IEnumerable<ProductResponseDto> ToResponseDtos(this IEnumerable<Product> products)
    {
        return products.Select(p => p.ToResponseDto());
    }
}