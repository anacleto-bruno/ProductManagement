using Microsoft.Extensions.Logging;
using Moq;
using ProductManagement.dtos;
using ProductManagement.functions;
using ProductManagement.models;
using ProductManagement.services;
using ProductManagement.validators;
using FluentAssertions;

namespace ProductManagement.Tests.Functions;

public class ProductFunctionsTests
{
    private readonly Mock<ILogger<ProductFunctions>> _mockLogger;
    private readonly Mock<IProductService> _mockProductService;
    private readonly CreateProductRequestValidator _createValidator;
    private readonly UpdateProductRequestValidator _updateValidator;  
    private readonly PaginationRequestValidator _paginationValidator;
    private readonly ProductFunctions _productFunctions;

    public ProductFunctionsTests()
    {
        _mockLogger = new Mock<ILogger<ProductFunctions>>();
        _mockProductService = new Mock<IProductService>();
        _createValidator = new CreateProductRequestValidator();
        _updateValidator = new UpdateProductRequestValidator();
        _paginationValidator = new PaginationRequestValidator();

        _productFunctions = new ProductFunctions(
            _mockLogger.Object,
            _mockProductService.Object,
            _createValidator,
            _updateValidator,
            _paginationValidator);
    }

    #region Service Integration Tests

    [Fact]
    public async Task ProductService_GetByIdAsync_Success_CallsServiceCorrectly()
    {
        // Arrange
        var productId = 1;
        var expectedResult = Result<ProductResponseDto>.Success(new ProductResponseDto
        {
            Id = productId,
            Name = "Test Product",
            Sku = "TEST-001",
            Price = 10.99m
        });

        _mockProductService.Setup(s => s.GetByIdAsync(productId))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _mockProductService.Object.GetByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data!.Id.Should().Be(productId);
        result.Data.Name.Should().Be("Test Product");
        _mockProductService.Verify(s => s.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task ProductService_GetByIdAsync_Failure_ReturnsFailureResult()
    {
        // Arrange
        var productId = 999;
        var expectedResult = Result<ProductResponseDto>.Failure("Product not found");

        _mockProductService.Setup(s => s.GetByIdAsync(productId))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _mockProductService.Object.GetByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Product not found");
        _mockProductService.Verify(s => s.GetByIdAsync(productId), Times.Once);
    }

    #endregion

    #region Pagination Service Tests

    [Fact]
    public async Task ProductService_GetPagedAsync_ValidRequest_ReturnsPagedResults()
    {
        // Arrange
        var request = new PaginationRequestDto
        {
            Page = 1,
            PageSize = 20,
            SearchTerm = "laptop"
        };

        var expectedResult = Result<PagedResultDto<ProductSummaryDto>>.Success(new PagedResultDto<ProductSummaryDto>
        {
            Data = new List<ProductSummaryDto>
            {
                new() { Id = 1, Name = "Gaming Laptop", Sku = "LAP-001", Price = 999.99m }
            },
            TotalCount = 1,
            Page = 1,
            PageSize = 20,
            TotalPages = 1
        });

        _mockProductService.Setup(s => s.GetPagedAsync(It.IsAny<PaginationRequestDto>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _mockProductService.Object.GetPagedAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data!.Data.Should().HaveCount(1);
        result.Data.Data.First().Name.Should().Be("Gaming Laptop");
        _mockProductService.Verify(s => s.GetPagedAsync(It.IsAny<PaginationRequestDto>()), Times.Once);
    }

    [Fact]
    public async Task PaginationValidator_ValidRequest_PassesValidation()
    {
        // Arrange
        var validRequest = new PaginationRequestDto
        {
            Page = 1,
            PageSize = 20,
            MinPrice = 10,
            MaxPrice = 100
        };

        // Act
        var result = await _paginationValidator.ValidateAsync(validRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task PaginationValidator_InvalidPage_FailsValidation()
    {
        // Arrange
        var invalidRequest = new PaginationRequestDto
        {
            Page = 0, // Invalid
            PageSize = 20
        };

        // Act
        var result = await _paginationValidator.ValidateAsync(invalidRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.PropertyName == "Page");
    }

    [Fact]
    public async Task PaginationValidator_InvalidPageSize_FailsValidation()
    {
        // Arrange
        var invalidRequest = new PaginationRequestDto
        {
            Page = 1,
            PageSize = 101 // Invalid - too large
        };

        // Act
        var result = await _paginationValidator.ValidateAsync(invalidRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.PropertyName == "PageSize");
    }

    #endregion

    #region Create Product Tests

    [Fact]
    public async Task ProductService_CreateAsync_ValidProduct_ReturnsCreatedProduct()
    {
        // Arrange
        var createRequest = new CreateProductRequestDto
        {
            Name = "New Gaming Laptop",
            Sku = "LAP-002",
            Price = 1299.99m,
            Description = "High-performance gaming laptop",
            Brand = "TechBrand",
            Model = "Gaming Pro",
            Category = "Electronics"
        };

        var expectedResult = Result<ProductResponseDto>.Success(new ProductResponseDto
        {
            Id = 1,
            Name = createRequest.Name,
            Sku = createRequest.Sku,
            Price = createRequest.Price,
            Description = createRequest.Description,
            Brand = createRequest.Brand,
            Model = createRequest.Model,
            Category = createRequest.Category
        });

        _mockProductService.Setup(s => s.CreateAsync(It.IsAny<CreateProductRequestDto>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _mockProductService.Object.CreateAsync(createRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be(createRequest.Name);
        result.Data.Sku.Should().Be(createRequest.Sku);
        result.Data.Price.Should().Be(createRequest.Price);
        _mockProductService.Verify(s => s.CreateAsync(It.IsAny<CreateProductRequestDto>()), Times.Once);
    }

    [Fact]
    public async Task CreateProductValidator_ValidProduct_PassesValidation()
    {
        // Arrange
        var validRequest = new CreateProductRequestDto
        {
            Name = "Valid Product",
            Sku = "VALID-001",
            Price = 29.99m,
            Description = "A valid product",
            Brand = "TestBrand",
            Model = "TestModel",
            Category = "TestCategory"
        };

        // Act
        var result = await _createValidator.ValidateAsync(validRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateProductValidator_InvalidName_FailsValidation()
    {
        // Arrange
        var invalidRequest = new CreateProductRequestDto
        {
            Name = "", // Invalid - empty
            Sku = "VALID-001",
            Price = 29.99m
        };

        // Act
        var result = await _createValidator.ValidateAsync(invalidRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task CreateProductValidator_InvalidPrice_FailsValidation()
    {
        // Arrange
        var invalidRequest = new CreateProductRequestDto
        {
            Name = "Valid Product",
            Sku = "VALID-001",
            Price = -10 // Invalid - negative
        };

        // Act
        var result = await _createValidator.ValidateAsync(invalidRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    #endregion

    #region Update Product Tests

    [Fact]
    public async Task ProductService_UpdateAsync_ValidUpdate_ReturnsUpdatedProduct()
    {
        // Arrange
        var productId = 1;
        var updateRequest = new UpdateProductRequestDto
        {
            Name = "Updated Gaming Laptop",
            Price = 1399.99m,
            Description = "Updated high-performance gaming laptop"
        };

        var expectedResult = Result<ProductResponseDto>.Success(new ProductResponseDto
        {
            Id = productId,
            Name = updateRequest.Name,
            Sku = "LAP-001",
            Price = updateRequest.Price,
            Description = updateRequest.Description
        });

        _mockProductService.Setup(s => s.UpdateAsync(productId, It.IsAny<UpdateProductRequestDto>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _mockProductService.Object.UpdateAsync(productId, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data!.Id.Should().Be(productId);
        result.Data.Name.Should().Be(updateRequest.Name);
        result.Data.Price.Should().Be(updateRequest.Price);
        _mockProductService.Verify(s => s.UpdateAsync(productId, It.IsAny<UpdateProductRequestDto>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProductValidator_ValidUpdate_PassesValidation()
    {
        // Arrange
        var validRequest = new UpdateProductRequestDto
        {
            Name = "Updated Product Name",
            Model = "Updated Model",
            Brand = "Updated Brand",
            Sku = "UPD-001",
            Price = 39.99m,
            Description = "Updated description"
        };

        // Act
        var result = await _updateValidator.ValidateAsync(validRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateProductValidator_InvalidPrice_FailsValidation()
    {
        // Arrange
        var invalidRequest = new UpdateProductRequestDto
        {
            Name = "Valid Name",
            Price = -5 // Invalid - negative
        };

        // Act
        var result = await _updateValidator.ValidateAsync(invalidRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Fact]
    public async Task ProductService_UpdateAsync_ProductNotFound_ReturnsFailure()
    {
        // Arrange
        var productId = 999;
        var updateRequest = new UpdateProductRequestDto
        {
            Name = "Updated Product",
            Price = 25.99m
        };

        var expectedResult = Result<ProductResponseDto>.Failure("Product not found");

        _mockProductService.Setup(s => s.UpdateAsync(productId, It.IsAny<UpdateProductRequestDto>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _mockProductService.Object.UpdateAsync(productId, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Product not found");
        _mockProductService.Verify(s => s.UpdateAsync(productId, It.IsAny<UpdateProductRequestDto>()), Times.Once);
    }

    #endregion

    #region Delete Product Tests

    [Fact]
    public async Task ProductService_DeleteAsync_ValidId_ReturnsSuccess()
    {
        // Arrange
        var productId = 1;
        var expectedResult = Result.Success();

        _mockProductService.Setup(s => s.DeleteAsync(productId))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _mockProductService.Object.DeleteAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        _mockProductService.Verify(s => s.DeleteAsync(productId), Times.Once);
    }

    [Fact]
    public async Task ProductService_DeleteAsync_InvalidId_ReturnsFailure()
    {
        // Arrange
        var productId = 999;
        var expectedResult = Result.Failure("Product not found");

        _mockProductService.Setup(s => s.DeleteAsync(productId))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _mockProductService.Object.DeleteAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Product not found");
        _mockProductService.Verify(s => s.DeleteAsync(productId), Times.Once);
    }

    [Fact]
    public async Task ProductService_DeleteAsync_ServiceException_ThrowsException()
    {
        // Arrange
        var productId = 1;
        _mockProductService.Setup(s => s.DeleteAsync(productId))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _mockProductService.Object.DeleteAsync(productId));
        
        _mockProductService.Verify(s => s.DeleteAsync(productId), Times.Once);
    }

    #endregion

    #region Seed Products Tests

    [Fact]
    public async Task ProductService_SeedAsync_DefaultCount_ReturnsSeededProducts()
    {
        // Arrange
        var defaultCount = 100;
        var seededProducts = new List<ProductResponseDto>
        {
            new() { Id = 1, Name = "Seeded Gaming Laptop", Sku = "SEED-001", Price = 999.99m },
            new() { Id = 2, Name = "Seeded Office Mouse", Sku = "SEED-002", Price = 29.99m }
        };

        var expectedResult = Result<List<ProductResponseDto>>.Success(seededProducts);

        _mockProductService.Setup(s => s.SeedAsync(defaultCount))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _mockProductService.Object.SeedAsync(defaultCount);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data!.Should().Contain(p => p.Name == "Seeded Gaming Laptop");
        _mockProductService.Verify(s => s.SeedAsync(defaultCount), Times.Once);
    }

    [Fact]
    public async Task ProductService_SeedAsync_CustomCount_ReturnsSpecifiedCount()
    {
        // Arrange
        var customCount = 25;
        var seededProducts = Enumerable.Range(1, customCount)
            .Select(i => new ProductResponseDto
            {
                Id = i,
                Name = $"Seeded Product {i}",
                Sku = $"SEED-{i:D3}",
                Price = 10.99m + i
            }).ToList();

        var expectedResult = Result<List<ProductResponseDto>>.Success(seededProducts);

        _mockProductService.Setup(s => s.SeedAsync(customCount))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _mockProductService.Object.SeedAsync(customCount);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(customCount);
        _mockProductService.Verify(s => s.SeedAsync(customCount), Times.Once);
    }

    [Fact]
    public async Task ProductService_SeedAsync_InvalidCount_ReturnsFailure()
    {
        // Arrange
        var invalidCount = -5;
        var expectedResult = Result<List<ProductResponseDto>>.Failure("Count must be between 1 and 10,000");

        _mockProductService.Setup(s => s.SeedAsync(invalidCount))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _mockProductService.Object.SeedAsync(invalidCount);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Count must be between");
        _mockProductService.Verify(s => s.SeedAsync(invalidCount), Times.Once);
    }

    [Fact]
    public async Task ProductService_SeedAsync_ServiceException_ThrowsException()
    {
        // Arrange
        var count = 100;
        _mockProductService.Setup(s => s.SeedAsync(count))
            .ThrowsAsync(new InvalidOperationException("Database seeding failed"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _mockProductService.Object.SeedAsync(count));
        
        _mockProductService.Verify(s => s.SeedAsync(count), Times.Once);
    }

    #endregion
}