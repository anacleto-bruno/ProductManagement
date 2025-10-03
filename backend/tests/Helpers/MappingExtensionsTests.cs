using FluentAssertions;
using ProductManagement.helpers;
using ProductManagement.dtos;
using ProductManagement.entities;

namespace ProductManagement.Tests.Helpers;

/// <summary>
/// Tests for MappingExtensions static methods
/// </summary>
public class MappingExtensionsTests
{
    [Fact]
    public void ToResponseDto_ValidProduct_MapsCorrectly()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            Name = "Test Product",
            Description = "Test Description",
            Model = "TestModel",
            Brand = "TestBrand",
            Sku = "TEST-SKU-001",
            Price = 99.99m,
            Category = "Electronics",
            CreatedAt = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2023, 1, 2, 12, 0, 0, DateTimeKind.Utc),
            ProductColors = new List<ProductColor>
            {
                new() { Color = new Color { Id = 1, Name = "Red", HexCode = "#FF0000" } },
                new() { Color = new Color { Id = 2, Name = "Blue", HexCode = "#0000FF" } }
            },
            ProductSizes = new List<ProductSize>
            {
                new() { Size = new Size { Id = 1, Name = "Small", Code = "S", SortOrder = 1 } },
                new() { Size = new Size { Id = 2, Name = "Medium", Code = "M", SortOrder = 2 } }
            }
        };

        // Act
        var result = product.ToResponseDto();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Test Product");
        result.Description.Should().Be("Test Description");
        result.Model.Should().Be("TestModel");
        result.Brand.Should().Be("TestBrand");
        result.Sku.Should().Be("TEST-SKU-001");
        result.Price.Should().Be(99.99m);
        result.Category.Should().Be("Electronics");
        result.CreatedAt.Should().Be(new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc));
        result.UpdatedAt.Should().Be(new DateTime(2023, 1, 2, 12, 0, 0, DateTimeKind.Utc));
        
        result.Colors.Should().HaveCount(2);
        result.Colors[0].Id.Should().Be(1);
        result.Colors[0].Name.Should().Be("Red");
        result.Colors[0].HexCode.Should().Be("#FF0000");
        result.Colors[1].Id.Should().Be(2);
        result.Colors[1].Name.Should().Be("Blue");
        result.Colors[1].HexCode.Should().Be("#0000FF");
        
        result.Sizes.Should().HaveCount(2);
        result.Sizes[0].Id.Should().Be(1);
        result.Sizes[0].Name.Should().Be("Small");
        result.Sizes[0].Code.Should().Be("S");
        result.Sizes[0].SortOrder.Should().Be(1);
        result.Sizes[1].Id.Should().Be(2);
        result.Sizes[1].Name.Should().Be("Medium");
        result.Sizes[1].Code.Should().Be("M");
        result.Sizes[1].SortOrder.Should().Be(2);
    }

    [Fact]
    public void ToResponseDto_ProductWithEmptyCollections_MapsCorrectly()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            Name = "Simple Product",
            Description = "Simple Description",
            Model = "SimpleModel",
            Brand = "SimpleBrand",
            Sku = "SIMPLE-001",
            Price = 50.00m,
            Category = "Basic",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ProductColors = new List<ProductColor>(),
            ProductSizes = new List<ProductSize>()
        };

        // Act
        var result = product.ToResponseDto();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Simple Product");
        result.Colors.Should().BeEmpty();
        result.Sizes.Should().BeEmpty();
    }

    [Fact]
    public void ToEntity_ValidCreateDto_MapsCorrectly()
    {
        // Arrange
        var dto = new CreateProductRequestDto
        {
            Name = "New Product",
            Description = "New Description",
            Model = "NewModel",
            Brand = "NewBrand",
            Sku = "NEW-SKU-001",
            Price = 199.99m,
            Category = "Technology",
            ColorIds = new List<int> { 1, 2 },
            SizeIds = new List<int> { 1 }
        };

        var beforeCreate = DateTime.UtcNow;

        // Act
        var result = dto.ToEntity();

        var afterCreate = DateTime.UtcNow;

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(0); // New entity should have default ID
        result.Name.Should().Be("New Product");
        result.Description.Should().Be("New Description");
        result.Model.Should().Be("NewModel");
        result.Brand.Should().Be("NewBrand");
        result.Sku.Should().Be("NEW-SKU-001");
        result.Price.Should().Be(199.99m);
        result.Category.Should().Be("Technology");
        
        // Verify timestamps are set to current UTC time (within reasonable tolerance)
        result.CreatedAt.Should().BeCloseTo(beforeCreate, TimeSpan.FromSeconds(1));
        result.UpdatedAt.Should().BeCloseTo(beforeCreate, TimeSpan.FromSeconds(1));
        result.CreatedAt.Should().BeCloseTo(result.UpdatedAt, TimeSpan.FromMilliseconds(10));
        
        // Collections should be empty (relationships are added separately)
        result.ProductColors.Should().BeEmpty();
        result.ProductSizes.Should().BeEmpty();
    }

    [Fact]
    public void UpdateFromDto_ValidUpdateDto_UpdatesProductCorrectly()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            Name = "Original Product",
            Description = "Original Description",
            Model = "OriginalModel",
            Brand = "OriginalBrand",
            Sku = "ORIGINAL-001",
            Price = 100.00m,
            Category = "Original",
            CreatedAt = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc)
        };

        var updateDto = new UpdateProductRequestDto
        {
            Name = "Updated Product",
            Description = "Updated Description",
            Model = "UpdatedModel",
            Brand = "UpdatedBrand",
            Sku = "UPDATED-001",
            Price = 150.00m,
            Category = "Updated",
            ColorIds = new List<int> { 1 },
            SizeIds = new List<int> { 1 }
        };

        var beforeUpdate = DateTime.UtcNow;

        // Act
        product.UpdateFromDto(updateDto);

        var afterUpdate = DateTime.UtcNow;

        // Assert
        product.Id.Should().Be(1); // ID should not change
        product.Name.Should().Be("Updated Product");
        product.Description.Should().Be("Updated Description");
        product.Model.Should().Be("UpdatedModel");
        product.Brand.Should().Be("UpdatedBrand");
        product.Sku.Should().Be("UPDATED-001");
        product.Price.Should().Be(150.00m);
        product.Category.Should().Be("Updated");
        
        // CreatedAt should remain unchanged
        product.CreatedAt.Should().Be(new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc));
        
        // UpdatedAt should be set to current UTC time
        product.UpdatedAt.Should().BeOnOrAfter(beforeUpdate).And.BeOnOrBefore(afterUpdate);
    }

    [Fact]
    public void ToDto_ValidColor_MapsCorrectly()
    {
        // Arrange
        var color = new Color
        {
            Id = 1,
            Name = "Crimson Red",
            HexCode = "#DC143C"
        };

        // Act
        var result = color.ToDto();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Crimson Red");
        result.HexCode.Should().Be("#DC143C");
    }

    [Fact]
    public void ToDto_ValidSize_MapsCorrectly()
    {
        // Arrange
        var size = new Size
        {
            Id = 1,
            Name = "Extra Large",
            Code = "XL",
            SortOrder = 5
        };

        // Act
        var result = size.ToDto();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Extra Large");
        result.Code.Should().Be("XL");
        result.SortOrder.Should().Be(5);
    }

    [Fact]
    public void ToResponseDtos_ValidProductCollection_MapsCorrectly()
    {
        // Arrange
        var products = new List<Product>
        {
            new()
            {
                Id = 1,
                Name = "Product 1",
                Description = "Description 1",
                Model = "Model1",
                Brand = "Brand1",
                Sku = "SKU-001",
                Price = 10.00m,
                Category = "Category1",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ProductColors = new List<ProductColor>(),
                ProductSizes = new List<ProductSize>()
            },
            new()
            {
                Id = 2,
                Name = "Product 2",
                Description = "Description 2",
                Model = "Model2",
                Brand = "Brand2",
                Sku = "SKU-002",
                Price = 20.00m,
                Category = "Category2",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ProductColors = new List<ProductColor>(),
                ProductSizes = new List<ProductSize>()
            }
        };

        // Act
        var result = products.ToResponseDtos().ToList();

        // Assert
        result.Should().HaveCount(2);
        
        result[0].Id.Should().Be(1);
        result[0].Name.Should().Be("Product 1");
        result[0].Sku.Should().Be("SKU-001");
        result[0].Price.Should().Be(10.00m);
        
        result[1].Id.Should().Be(2);
        result[1].Name.Should().Be("Product 2");
        result[1].Sku.Should().Be("SKU-002");
        result[1].Price.Should().Be(20.00m);
    }

    [Fact]
    public void ToResponseDtos_EmptyCollection_ReturnsEmptyCollection()
    {
        // Arrange
        var products = new List<Product>();

        // Act
        var result = products.ToResponseDtos().ToList();

        // Assert
        result.Should().BeEmpty();
    }
}