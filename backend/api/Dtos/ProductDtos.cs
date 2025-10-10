namespace ProductManagement.dtos;

public record ProductResponseDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Model { get; init; } = string.Empty;
    public string Brand { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string? Category { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public List<ColorDto> Colors { get; init; } = new();
    public List<SizeDto> Sizes { get; init; } = new();
}

public record ProductSummaryDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
    public string Brand { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public List<ColorDto> Colors { get; init; } = new();
    public List<SizeDto> Sizes { get; init; } = new();
}

public record CreateProductRequestDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Model { get; init; } = string.Empty;
    public string Brand { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string? Category { get; init; }
    public List<int> ColorIds { get; init; } = new();
    public List<int> SizeIds { get; init; } = new();
}

public record UpdateProductRequestDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Model { get; init; } = string.Empty;
    public string Brand { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string? Category { get; init; }
    public List<int> ColorIds { get; init; } = new();
    public List<int> SizeIds { get; init; } = new();
}

public record ColorDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? HexCode { get; init; }
}

public record SizeDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Code { get; init; }
    public int? SortOrder { get; init; }
}