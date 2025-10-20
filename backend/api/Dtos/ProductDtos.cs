namespace ProductManagement.dtos;

// Base records for common properties
public abstract record BaseEntityDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

public abstract record ProductBaseDto : BaseEntityDto
{
    public string? Description { get; init; }
    public string Model { get; init; } = string.Empty;
    public string Brand { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string? Category { get; init; }
}

public abstract record ProductRequestBaseDto : ProductBaseDto
{
    public List<int> ColorIds { get; init; } = new();
    public List<int> SizeIds { get; init; } = new();
}

// Concrete DTOs
public record ProductResponseDto : ProductBaseDto
{
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public List<ColorDto> Colors { get; init; } = new();
    public List<SizeDto> Sizes { get; init; } = new();
}

public record ProductSummaryDto : ProductBaseDto
{
    public List<ColorDto> Colors { get; init; } = new();
    public List<SizeDto> Sizes { get; init; } = new();
}

public record CreateProductRequestDto : ProductRequestBaseDto;

public record UpdateProductRequestDto : ProductRequestBaseDto;

// Base DTO for entities with Id and Name

public record ColorDto : BaseEntityDto
{
    public string? HexCode { get; init; }
}

public record SizeDto : BaseEntityDto
{
    public string? Code { get; init; }
    public int? SortOrder { get; init; }
}