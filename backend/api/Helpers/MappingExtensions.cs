using System.Linq.Expressions;
using ProductManagement.dtos;
using ProductManagement.entities;

namespace ProductManagement.helpers;

public static class MappingExtensions
{
    // ===========================================
    // Product Entity → DTO Mappings
    // ===========================================

    /// <summary>
    /// Projection expression for ProductResponseDto (use in LINQ queries for performance)
    /// </summary>
    public static Expression<Func<Product, ProductResponseDto>> ToResponseDtoExpression =>
        p => new ProductResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Model = p.Model,
            Brand = p.Brand,
            Sku = p.Sku,
            Price = p.Price,
            Category = p.Category,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            Colors = p.ProductColors.Select(pc => new ColorDto
            {
                Id = pc.Color.Id,
                Name = pc.Color.Name,
                HexCode = pc.Color.HexCode
            }).ToList(),
            Sizes = p.ProductSizes.Select(ps => new SizeDto
            {
                Id = ps.Size.Id,
                Name = ps.Size.Name,
                Code = ps.Size.Code,
                SortOrder = ps.Size.SortOrder
            }).ToList()
        };

    /// <summary>
    /// Projection expression for ProductSummaryDto (use in LINQ queries for performance)
    /// </summary>
    public static Expression<Func<Product, ProductSummaryDto>> ToSummaryDtoExpression =>
        p => new ProductSummaryDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Model = p.Model,
            Brand = p.Brand,
            Sku = p.Sku,
            Price = p.Price,
            Category = p.Category,
            Colors = p.ProductColors.Select(pc => new ColorDto
            {
                Id = pc.Color.Id,
                Name = pc.Color.Name,
                HexCode = pc.Color.HexCode
            }).ToList(),
            Sizes = p.ProductSizes.Select(ps => new SizeDto
            {
                Id = ps.Size.Id,
                Name = ps.Size.Name,
                Code = ps.Size.Code,
                SortOrder = ps.Size.SortOrder
            }).ToList()
        };

    /// <summary>
    /// Maps Product entity to ProductResponseDto (use for in-memory objects)
    /// </summary>
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
            Colors = product.ProductColors?.Select(pc => pc.Color.ToDto()).ToList() ?? new List<ColorDto>(),
            Sizes = product.ProductSizes?.Select(ps => ps.Size.ToDto()).ToList() ?? new List<SizeDto>()
        };
    }

    /// <summary>
    /// Maps Product entity to ProductSummaryDto (use for in-memory objects)
    /// </summary>
    public static ProductSummaryDto ToSummaryDto(this Product product)
    {
        return new ProductSummaryDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Model = product.Model,
            Brand = product.Brand,
            Sku = product.Sku,
            Price = product.Price,
            Category = product.Category,
            Colors = product.ProductColors?.Select(pc => pc.Color.ToDto()).ToList() ?? new List<ColorDto>(),
            Sizes = product.ProductSizes?.Select(ps => ps.Size.ToDto()).ToList() ?? new List<SizeDto>()
        };
    }

    /// <summary>
    /// Maps collection of Products to ProductResponseDtos
    /// </summary>
    public static IEnumerable<ProductResponseDto> ToResponseDtos(this IEnumerable<Product> products)
    {
        return products.Select(p => p.ToResponseDto());
    }

    /// <summary>
    /// Maps collection of Products to ProductSummaryDtos
    /// </summary>
    public static IEnumerable<ProductSummaryDto> ToSummaryDtos(this IEnumerable<Product> products)
    {
        return products.Select(p => p.ToSummaryDto());
    }

    // ===========================================
    // DTO → Product Entity Mappings
    // ===========================================

    /// <summary>
    /// Maps CreateProductRequestDto to new Product entity
    /// </summary>
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

    /// <summary>
    /// Updates existing Product entity from UpdateProductRequestDto
    /// </summary>
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

    // ===========================================
    // Color Entity ↔ DTO Mappings
    // ===========================================

    /// <summary>
    /// Projection expression for ColorDto (use in LINQ queries)
    /// </summary>
    public static Expression<Func<Color, ColorDto>> ToColorDtoExpression =>
        c => new ColorDto
        {
            Id = c.Id,
            Name = c.Name,
            HexCode = c.HexCode
        };

    /// <summary>
    /// Maps Color entity to ColorDto (use for in-memory objects)
    /// </summary>
    public static ColorDto ToDto(this Color color)
    {
        return new ColorDto
        {
            Id = color.Id,
            Name = color.Name,
            HexCode = color.HexCode
        };
    }

    /// <summary>
    /// Maps collection of Colors to ColorDtos
    /// </summary>
    public static IEnumerable<ColorDto> ToDtos(this IEnumerable<Color> colors)
    {
        return colors.Select(c => c.ToDto());
    }

    // ===========================================
    // Size Entity ↔ DTO Mappings
    // ===========================================

    /// <summary>
    /// Projection expression for SizeDto (use in LINQ queries)
    /// </summary>
    public static Expression<Func<Size, SizeDto>> ToSizeDtoExpression =>
        s => new SizeDto
        {
            Id = s.Id,
            Name = s.Name,
            Code = s.Code,
            SortOrder = s.SortOrder
        };

    /// <summary>
    /// Maps Size entity to SizeDto (use for in-memory objects)
    /// </summary>
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

    /// <summary>
    /// Maps collection of Sizes to SizeDtos
    /// </summary>
    public static IEnumerable<SizeDto> ToDtos(this IEnumerable<Size> sizes)
    {
        return sizes.Select(s => s.ToDto());
    }
}