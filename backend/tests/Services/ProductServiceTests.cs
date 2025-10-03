using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using ProductManagement.services;
using ProductManagement.infrastructure.repositories;
using ProductManagement.dtos;
using ProductManagement.entities;

namespace ProductManagement.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ILogger<ProductService>> _mockLogger;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockLogger = new Mock<ILogger<ProductService>>();
        _productService = new ProductService(_mockProductRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsSuccessResult()
    {
        // Arrange
        var productId = 1;
        var expectedProduct = new ProductResponseDto
        {
            Id = productId,
            Name = "Test Product",
            Description = "Test Description",
            Model = "Model123",
            Brand = "TestBrand",
            Sku = "SKU123",
            Price = 99.99m,
            Category = "Electronics",
            Colors = new List<ColorDto>(),
            Sizes = new List<SizeDto>()
        };

        _mockProductRepository
            .Setup(x => x.GetProductDtoByIdAsync(productId))
            .ReturnsAsync(expectedProduct);

        // Act
        var result = await _productService.GetByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(expectedProduct);
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_InvalidId_ReturnsFailureResult()
    {
        // Arrange
        var invalidId = -1;

        // Act
        var result = await _productService.GetByIdAsync(invalidId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.ErrorMessage.Should().Be("Invalid product ID");
    }

    [Fact]
    public async Task GetByIdAsync_ProductNotFound_ReturnsFailureResult()
    {
        // Arrange
        var productId = 999;
        _mockProductRepository
            .Setup(x => x.GetProductDtoByIdAsync(productId))
            .ReturnsAsync((ProductResponseDto?)null);

        // Act
        var result = await _productService.GetByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.ErrorMessage.Should().Be("Product not found");
    }

    [Fact]
    public async Task GetPagedAsync_ValidRequest_ReturnsSuccessResult()
    {
        // Arrange
        var request = new PaginationRequestDto
        {
            Page = 1,
            PageSize = 10,
            SearchTerm = "test"
        };

        var expectedResult = new PagedResultDto<ProductSummaryDto>
        {
            Data = new List<ProductSummaryDto>
            {
                new() { Id = 1, Name = "Product 1", Brand = "Brand A", Price = 10.00m },
                new() { Id = 2, Name = "Product 2", Brand = "Brand B", Price = 20.00m }
            },
            Page = 1,
            PageSize = 10,
            TotalCount = 2,
            TotalPages = 1
        };

        _mockProductRepository
            .Setup(x => x.GetProductDtosAsync(request))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _productService.GetPagedAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task CreateAsync_ValidProduct_ReturnsSuccessResult()
    {
        // Arrange
        var createRequest = new CreateProductRequestDto
        {
            Name = "New Product",
            Description = "New Description",
            Model = "NewModel",
            Brand = "NewBrand",
            Sku = "NEWSKU123",
            Price = 150.00m,
            Category = "Electronics",
            ColorIds = new List<int> { 1, 2 },
            SizeIds = new List<int> { 1 }
        };

        var createdProduct = new Product
        {
            Id = 1,
            Name = createRequest.Name,
            Description = createRequest.Description,
            Model = createRequest.Model,
            Brand = createRequest.Brand,
            Sku = createRequest.Sku,
            Price = createRequest.Price,
            Category = createRequest.Category
        };

        var expectedResponse = new ProductResponseDto
        {
            Id = 1,
            Name = createRequest.Name,
            Description = createRequest.Description,
            Model = createRequest.Model,
            Brand = createRequest.Brand,
            Sku = createRequest.Sku,
            Price = createRequest.Price,
            Category = createRequest.Category,
            Colors = new List<ColorDto>(),
            Sizes = new List<SizeDto>()
        };

        _mockProductRepository
            .Setup(x => x.ExistsBySkuAsync(It.Is<string>(s => s == createRequest.Sku), It.IsAny<int?>()))
            .ReturnsAsync(false);

        _mockProductRepository
            .Setup(x => x.GetColorsAsync())
            .ReturnsAsync(new List<Color>
            {
                new() { Id = 1, Name = "Red", HexCode = "#FF0000" },
                new() { Id = 2, Name = "Blue", HexCode = "#0000FF" }
            });

        _mockProductRepository
            .Setup(x => x.GetSizesAsync())
            .ReturnsAsync(new List<Size>
            {
                new() { Id = 1, Name = "Medium", Code = "M" }
            });

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
        result.Data.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task CreateAsync_DuplicateSku_ReturnsFailureResult()
    {
        // Arrange
        var createRequest = new CreateProductRequestDto
        {
            Name = "New Product",
            Sku = "EXISTING_SKU",
            Price = 100.00m
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
    public async Task UpdateAsync_ValidProduct_ReturnsSuccessResult()
    {
        // Arrange
        var productId = 1;
        var updateRequest = new UpdateProductRequestDto
        {
            Name = "Updated Product",
            Description = "Updated Description",
            Model = "UpdatedModel",
            Brand = "UpdatedBrand",
            Sku = "UPDATED_SKU",
            Price = 200.00m,
            Category = "Updated Category",
            ColorIds = new List<int> { 1 },
            SizeIds = new List<int> { 1 }
        };

        var existingProduct = new Product
        {
            Id = productId,
            Name = "Old Product",
            Sku = "OLD_SKU",
            Price = 100.00m
        };

        var expectedResponse = new ProductResponseDto
        {
            Id = productId,
            Name = updateRequest.Name,
            Description = updateRequest.Description,
            Model = updateRequest.Model,
            Brand = updateRequest.Brand,
            Sku = updateRequest.Sku,
            Price = updateRequest.Price,
            Category = updateRequest.Category,
            Colors = new List<ColorDto>(),
            Sizes = new List<SizeDto>()
        };

        _mockProductRepository
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);

        _mockProductRepository
            .Setup(x => x.ExistsBySkuAsync(It.Is<string>(s => s == updateRequest.Sku), It.Is<int>(id => id == productId)))
            .ReturnsAsync(false);

        _mockProductRepository
            .Setup(x => x.GetColorsAsync())
            .ReturnsAsync(new List<Color> { new() { Id = 1, Name = "Red", HexCode = "#FF0000" } });

        _mockProductRepository
            .Setup(x => x.GetSizesAsync())
            .ReturnsAsync(new List<Size> { new() { Id = 1, Name = "Medium", Code = "M" } });

        _mockProductRepository
            .Setup(x => x.GetProductDtoByIdAsync(productId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _productService.UpdateAsync(productId, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task UpdateAsync_ProductNotFound_ReturnsFailureResult()
    {
        // Arrange
        var productId = 999;
        var updateRequest = new UpdateProductRequestDto
        {
            Name = "Updated Product",
            Sku = "UPDATED_SKU",
            Price = 200.00m
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
    public async Task DeleteAsync_ValidId_ReturnsSuccessResult()
    {
        // Arrange
        var productId = 1;
        var existingProduct = new Product { Id = productId, Name = "Test Product" };

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

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
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
    public async Task SeedAsync_NoColorsOrSizes_ReturnsFailureResult()
    {
        // Arrange
        _mockProductRepository
            .Setup(x => x.GetColorsAsync())
            .ReturnsAsync(new List<Color>());

        _mockProductRepository
            .Setup(x => x.GetSizesAsync())
            .ReturnsAsync(new List<Size>());

        // Act
        var result = await _productService.SeedAsync(5);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.ErrorMessage.Should().Contain("Colors or sizes not available");
    }
}