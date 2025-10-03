using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using ProductManagement.services;
using ProductManagement.infrastructure.repositories;
using ProductManagement.dtos;
using ProductManagement.entities;

namespace ProductManagement.Tests.Services;

/// <summary>
/// Tests for ProductService color and size relationship handling
/// </summary>
public class ProductServiceRelationshipTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ILogger<ProductService>> _mockLogger;
    private readonly ProductService _productService;

    public ProductServiceRelationshipTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockLogger = new Mock<ILogger<ProductService>>();
        _productService = new ProductService(_mockProductRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidColorsAndSizes_AddsRelationships()
    {
        // Arrange
        var createRequest = new CreateProductRequestDto
        {
            Name = "Test Product",
            Description = "Test Description",
            Model = "TestModel",
            Brand = "TestBrand",
            Sku = "TEST-SKU-001",
            Price = 100m,
            Category = "Electronics",
            ColorIds = new List<int> { 1, 2 },
            SizeIds = new List<int> { 1, 2 }
        };

        var availableColors = new List<Color>
        {
            new() { Id = 1, Name = "Red" },
            new() { Id = 2, Name = "Blue" },
            new() { Id = 3, Name = "Green" } // Not requested
        };

        var availableSizes = new List<Size>
        {
            new() { Id = 1, Name = "Small" },
            new() { Id = 2, Name = "Medium" },
            new() { Id = 3, Name = "Large" } // Not requested
        };

        var createdProduct = new Product
        {
            Id = 1,
            Name = createRequest.Name,
            Sku = createRequest.Sku,
            ProductColors = new List<ProductColor>(),
            ProductSizes = new List<ProductSize>()
        };

        var expectedResponse = new ProductResponseDto
        {
            Id = 1,
            Name = createRequest.Name,
            Colors = new List<ColorDto>
            {
                new() { Id = 1, Name = "Red" },
                new() { Id = 2, Name = "Blue" }
            },
            Sizes = new List<SizeDto>
            {
                new() { Id = 1, Name = "Small" },
                new() { Id = 2, Name = "Medium" }
            }
        };

        _mockProductRepository
            .Setup(x => x.ExistsBySkuAsync(It.Is<string>(s => s == createRequest.Sku), It.IsAny<int?>()))
            .ReturnsAsync(false);

        _mockProductRepository
            .Setup(x => x.GetColorsAsync())
            .ReturnsAsync(availableColors);

        _mockProductRepository
            .Setup(x => x.GetSizesAsync())
            .ReturnsAsync(availableSizes);

        _mockProductRepository
            .Setup(x => x.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync(createdProduct);

        _mockProductRepository
            .Setup(x => x.GetProductDtoByIdAsync(1))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _productService.CreateAsync(createRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Colors.Should().HaveCount(2);
        result.Data.Sizes.Should().HaveCount(2);

        // Verify that AddAsync was called with product containing correct relationships
        _mockProductRepository.Verify(x => x.AddAsync(It.Is<Product>(p => 
            p.ProductColors.Count == 2 && 
            p.ProductSizes.Count == 2 &&
            p.ProductColors.Any(pc => pc.ColorId == 1) &&
            p.ProductColors.Any(pc => pc.ColorId == 2) &&
            p.ProductSizes.Any(ps => ps.SizeId == 1) &&
            p.ProductSizes.Any(ps => ps.SizeId == 2)
        )), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidColorIds_OnlyAddsValidColors()
    {
        // Arrange
        var createRequest = new CreateProductRequestDto
        {
            Name = "Test Product",
            Sku = "TEST-SKU-002",
            Price = 100m,
            Brand = "TestBrand",
            Category = "Electronics",
            ColorIds = new List<int> { 1, 999 }, // 999 is invalid
            SizeIds = new List<int>()
        };

        var availableColors = new List<Color>
        {
            new() { Id = 1, Name = "Red" }
            // Color ID 999 doesn't exist
        };

        var createdProduct = new Product { Id = 1, Name = createRequest.Name, Sku = createRequest.Sku };
        var expectedResponse = new ProductResponseDto { Id = 1, Name = createRequest.Name };

        _mockProductRepository.Setup(x => x.ExistsBySkuAsync(It.Is<string>(s => s == createRequest.Sku), It.IsAny<int?>())).ReturnsAsync(false);
        _mockProductRepository.Setup(x => x.GetColorsAsync()).ReturnsAsync(availableColors);
        _mockProductRepository.Setup(x => x.GetSizesAsync()).ReturnsAsync(new List<Size>());
        _mockProductRepository.Setup(x => x.AddAsync(It.IsAny<Product>())).ReturnsAsync(createdProduct);
        _mockProductRepository.Setup(x => x.GetProductDtoByIdAsync(1)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _productService.CreateAsync(createRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        // Verify only valid color was added
        _mockProductRepository.Verify(x => x.AddAsync(It.Is<Product>(p => 
            p.ProductColors.Count == 1 && 
            p.ProductColors.Any(pc => pc.ColorId == 1)
        )), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithEmptyColorAndSizeIds_CreatesProductWithoutRelationships()
    {
        // Arrange
        var createRequest = new CreateProductRequestDto
        {
            Name = "Test Product",
            Sku = "TEST-SKU-003",
            Price = 100m,
            Brand = "TestBrand",
            Category = "Electronics",
            ColorIds = new List<int>(),
            SizeIds = new List<int>()
        };

        var createdProduct = new Product { Id = 1, Name = createRequest.Name, Sku = createRequest.Sku };
        var expectedResponse = new ProductResponseDto { Id = 1, Name = createRequest.Name };

        _mockProductRepository.Setup(x => x.ExistsBySkuAsync(It.Is<string>(s => s == createRequest.Sku), It.IsAny<int?>())).ReturnsAsync(false);
        _mockProductRepository.Setup(x => x.GetColorsAsync()).ReturnsAsync(new List<Color>());
        _mockProductRepository.Setup(x => x.GetSizesAsync()).ReturnsAsync(new List<Size>());
        _mockProductRepository.Setup(x => x.AddAsync(It.IsAny<Product>())).ReturnsAsync(createdProduct);
        _mockProductRepository.Setup(x => x.GetProductDtoByIdAsync(1)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _productService.CreateAsync(createRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        // Verify no color/size relationships were added
        _mockProductRepository.Verify(x => x.AddAsync(It.Is<Product>(p => 
            p.ProductColors.Count == 0 && 
            p.ProductSizes.Count == 0
        )), Times.Once);

        // Verify that GetColorsAsync and GetSizesAsync were NOT called when no IDs provided
        _mockProductRepository.Verify(x => x.GetColorsAsync(), Times.Never);
        _mockProductRepository.Verify(x => x.GetSizesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithValidColorsAndSizes_UpdatesRelationships()
    {
        // Arrange
        var productId = 1;
        var updateRequest = new UpdateProductRequestDto
        {
            Name = "Updated Product",
            Sku = "UPDATED-SKU",
            Price = 150m,
            Brand = "UpdatedBrand",
            Category = "Electronics",
            ColorIds = new List<int> { 2, 3 }, // Different from original
            SizeIds = new List<int> { 1 }
        };

        var existingProduct = new Product
        {
            Id = productId,
            Name = "Original Product",
            Sku = "ORIGINAL-SKU",
            ProductColors = new List<ProductColor>
            {
                new() { ProductId = productId, ColorId = 1 } // Will be cleared and replaced
            },
            ProductSizes = new List<ProductSize>
            {
                new() { ProductId = productId, SizeId = 2 } // Will be cleared and replaced
            }
        };

        var availableColors = new List<Color>
        {
            new() { Id = 2, Name = "Blue" },
            new() { Id = 3, Name = "Green" }
        };

        var availableSizes = new List<Size>
        {
            new() { Id = 1, Name = "Small" }
        };

        var expectedResponse = new ProductResponseDto { Id = productId, Name = updateRequest.Name };

        _mockProductRepository.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(existingProduct);
        _mockProductRepository.Setup(x => x.ExistsBySkuAsync(It.Is<string>(s => s == updateRequest.Sku), It.Is<int?>(id => id == productId))).ReturnsAsync(false);
        _mockProductRepository.Setup(x => x.GetColorsAsync()).ReturnsAsync(availableColors);
        _mockProductRepository.Setup(x => x.GetSizesAsync()).ReturnsAsync(availableSizes);
        _mockProductRepository.Setup(x => x.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);
        _mockProductRepository.Setup(x => x.GetProductDtoByIdAsync(productId)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _productService.UpdateAsync(productId, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        // Verify relationships were cleared and updated
        existingProduct.ProductColors.Should().HaveCount(2);
        existingProduct.ProductSizes.Should().HaveCount(1);
        existingProduct.ProductColors.Should().Contain(pc => pc.ColorId == 2);
        existingProduct.ProductColors.Should().Contain(pc => pc.ColorId == 3);
        existingProduct.ProductSizes.Should().Contain(ps => ps.SizeId == 1);

        _mockProductRepository.Verify(x => x.UpdateAsync(existingProduct), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithEmptyColorAndSizeIds_ClearsAllRelationships()
    {
        // Arrange
        var productId = 1;
        var updateRequest = new UpdateProductRequestDto
        {
            Name = "Updated Product",
            Sku = "UPDATED-SKU",
            Price = 150m,
            Brand = "UpdatedBrand",
            Category = "Electronics",
            ColorIds = new List<int>(), // Empty - should clear all
            SizeIds = new List<int>()   // Empty - should clear all
        };

        var existingProduct = new Product
        {
            Id = productId,
            Name = "Original Product",
            Sku = "ORIGINAL-SKU",
            ProductColors = new List<ProductColor>
            {
                new() { ProductId = productId, ColorId = 1 },
                new() { ProductId = productId, ColorId = 2 }
            },
            ProductSizes = new List<ProductSize>
            {
                new() { ProductId = productId, SizeId = 1 },
                new() { ProductId = productId, SizeId = 2 }
            }
        };

        var expectedResponse = new ProductResponseDto { Id = productId, Name = updateRequest.Name };

        _mockProductRepository.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(existingProduct);
        _mockProductRepository.Setup(x => x.ExistsBySkuAsync(It.Is<string>(s => s == updateRequest.Sku), It.Is<int?>(id => id == productId))).ReturnsAsync(false);
        _mockProductRepository.Setup(x => x.GetColorsAsync()).ReturnsAsync(new List<Color>());
        _mockProductRepository.Setup(x => x.GetSizesAsync()).ReturnsAsync(new List<Size>());
        _mockProductRepository.Setup(x => x.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);
        _mockProductRepository.Setup(x => x.GetProductDtoByIdAsync(productId)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _productService.UpdateAsync(productId, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        // Verify all relationships were cleared
        existingProduct.ProductColors.Should().BeEmpty();
        existingProduct.ProductSizes.Should().BeEmpty();

        // Verify that GetColorsAsync and GetSizesAsync were NOT called when no IDs provided
        _mockProductRepository.Verify(x => x.GetColorsAsync(), Times.Never);
        _mockProductRepository.Verify(x => x.GetSizesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ValidProduct_CallsRepositoryDelete()
    {
        // Arrange
        var productId = 1;
        var existingProduct = new Product
        {
            Id = productId,
            Name = "Test Product",
            Sku = "TEST-SKU",
            Price = 100m
        };

        _mockProductRepository
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);

        _mockProductRepository
            .Setup(x => x.DeleteAsync(existingProduct))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _productService.DeleteAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();

        // Verify repository methods were called
        _mockProductRepository.Verify(x => x.GetByIdAsync(productId), Times.Once);
        _mockProductRepository.Verify(x => x.DeleteAsync(existingProduct), Times.Once);
    }
}