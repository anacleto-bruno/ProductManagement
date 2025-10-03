using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using ProductManagement.services;
using ProductManagement.infrastructure.repositories;
using ProductManagement.dtos;
using ProductManagement.entities;

namespace ProductManagement.Tests.Services;

/// <summary>
/// Tests for ProductService error scenarios and edge cases that increase coverage
/// </summary>
public class ProductServiceErrorScenariosTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ILogger<ProductService>> _mockLogger;
    private readonly ProductService _productService;

    public ProductServiceErrorScenariosTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockLogger = new Mock<ILogger<ProductService>>();
        _productService = new ProductService(_mockProductRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetByIdAsync_RepositoryThrowsException_ReturnsFailureResult()
    {
        // Arrange
        var productId = 1;
        _mockProductRepository
            .Setup(x => x.GetProductDtoByIdAsync(productId))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _productService.GetByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.ErrorMessage.Should().Be("An error occurred while retrieving the product");
        
        // Verify error was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error retrieving product with ID")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_RepositoryThrowsException_ReturnsFailureResult()
    {
        // Arrange
        var request = new PaginationRequestDto { Page = 1, PageSize = 10 };
        _mockProductRepository
            .Setup(x => x.GetProductDtosAsync(request))
            .ThrowsAsync(new InvalidOperationException("Query timeout"));

        // Act
        var result = await _productService.GetPagedAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.ErrorMessage.Should().Be("An error occurred while retrieving products");
        
        // Verify error was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error retrieving paginated products")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_DuplicateSku_ReturnsFailureResult()
    {
        // Arrange
        var createRequest = new CreateProductRequestDto
        {
            Name = "Test Product",
            Sku = "DUPLICATE-SKU",
            Price = 100m,
            Brand = "TestBrand",
            Category = "Electronics",
            ColorIds = new List<int>(),
            SizeIds = new List<int>()
        };

        _mockProductRepository
            .Setup(x => x.ExistsBySkuAsync(It.Is<string>(s => s == createRequest.Sku), It.IsAny<int?>()))
            .ReturnsAsync(true);

        // Act
        var result = await _productService.CreateAsync(createRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.ErrorMessage.Should().Be("A product with this SKU already exists");
    }

    [Fact]
    public async Task CreateAsync_RepositoryThrowsException_ReturnsFailureResult()
    {
        // Arrange
        var createRequest = new CreateProductRequestDto
        {
            Name = "Test Product",
            Sku = "NEW-SKU",
            Price = 100m,
            Brand = "TestBrand",
            Category = "Electronics",
            ColorIds = new List<int>(),
            SizeIds = new List<int>()
        };

        _mockProductRepository
            .Setup(x => x.ExistsBySkuAsync(It.Is<string>(s => s == createRequest.Sku), It.IsAny<int?>()))
            .ReturnsAsync(false);
        
        _mockProductRepository
            .Setup(x => x.GetColorsAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _productService.CreateAsync(createRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.ErrorMessage.Should().Be("An error occurred while creating the product");
        
        // Verify error was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error creating product")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ProductNotFound_ReturnsFailureResult()
    {
        // Arrange
        var productId = 999;
        var updateRequest = new UpdateProductRequestDto
        {
            Name = "Updated Product",
            Sku = "UPDATED-SKU",
            Price = 150m,
            Brand = "UpdatedBrand",
            Category = "Electronics",
            ColorIds = new List<int>(),
            SizeIds = new List<int>()
        };

        _mockProductRepository
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.UpdateAsync(productId, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.ErrorMessage.Should().Be("Product not found");
    }

    [Fact]
    public async Task UpdateAsync_DuplicateSku_ReturnsFailureResult()
    {
        // Arrange
        var productId = 1;
        var updateRequest = new UpdateProductRequestDto
        {
            Name = "Updated Product",
            Sku = "DUPLICATE-SKU",
            Price = 150m,
            Brand = "UpdatedBrand",
            Category = "Electronics",
            ColorIds = new List<int>(),
            SizeIds = new List<int>()
        };

        var existingProduct = new Product { Id = productId, Name = "Existing", Sku = "OLD-SKU" };

        _mockProductRepository
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);

        _mockProductRepository
            .Setup(x => x.ExistsBySkuAsync(It.Is<string>(s => s == updateRequest.Sku), It.Is<int?>(id => id == productId)))
            .ReturnsAsync(true);

        // Act
        var result = await _productService.UpdateAsync(productId, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.ErrorMessage.Should().Be("A product with this SKU already exists");
    }

    [Fact]
    public async Task UpdateAsync_RepositoryThrowsException_ReturnsFailureResult()
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
            ColorIds = new List<int> { 1 }, // Non-empty to trigger GetColorsAsync
            SizeIds = new List<int>()
        };

        var existingProduct = new Product { Id = productId, Name = "Existing", Sku = "OLD-SKU" };

        _mockProductRepository
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);

        _mockProductRepository
            .Setup(x => x.ExistsBySkuAsync(It.Is<string>(s => s == updateRequest.Sku), It.Is<int?>(id => id == productId)))
            .ReturnsAsync(false);

        _mockProductRepository
            .Setup(x => x.GetColorsAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _productService.UpdateAsync(productId, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.ErrorMessage.Should().Be("An error occurred while updating the product");
        
        // Verify error was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error updating product with ID")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ProductNotFound_ReturnsFailureResult()
    {
        // Arrange
        var productId = 999;
        _mockProductRepository
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.DeleteAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Product not found");
    }

    [Fact]
    public async Task DeleteAsync_RepositoryThrowsException_ReturnsFailureResult()
    {
        // Arrange
        var productId = 1;
        var existingProduct = new Product { Id = productId, Name = "Test Product" };

        _mockProductRepository
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);

        _mockProductRepository
            .Setup(x => x.DeleteAsync(existingProduct))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _productService.DeleteAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("An error occurred while deleting the product");
        
        // Verify error was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error deleting product with ID")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(10001)]
    public async Task SeedAsync_InvalidCount_ReturnsFailureResult(int invalidCount)
    {
        // Act
        var result = await _productService.SeedAsync(invalidCount);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.ErrorMessage.Should().Be("Count must be between 1 and 10,000");
    }

    [Fact]
    public async Task SeedAsync_NoColorsAvailable_ReturnsFailureResult()
    {
        // Arrange
        _mockProductRepository
            .Setup(x => x.GetColorsAsync())
            .ReturnsAsync(new List<Color>());

        _mockProductRepository
            .Setup(x => x.GetSizesAsync())
            .ReturnsAsync(new List<Size> { new() { Id = 1, Name = "Medium" } });

        // Act
        var result = await _productService.SeedAsync(5);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.ErrorMessage.Should().Be("Colors or sizes not available. Please ensure database is properly seeded with reference data.");
    }

    [Fact]
    public async Task SeedAsync_NoSizesAvailable_ReturnsFailureResult()
    {
        // Arrange
        _mockProductRepository
            .Setup(x => x.GetColorsAsync())
            .ReturnsAsync(new List<Color> { new() { Id = 1, Name = "Red" } });

        _mockProductRepository
            .Setup(x => x.GetSizesAsync())
            .ReturnsAsync(new List<Size>());

        // Act
        var result = await _productService.SeedAsync(5);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.ErrorMessage.Should().Be("Colors or sizes not available. Please ensure database is properly seeded with reference data.");
    }

    [Fact]
    public async Task SeedAsync_RepositoryThrowsException_ReturnsFailureResult()
    {
        // Arrange
        _mockProductRepository
            .Setup(x => x.GetColorsAsync())
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _productService.SeedAsync(5);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.ErrorMessage.Should().Be("An error occurred while seeding products");
        
        // Verify error was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error seeding")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}