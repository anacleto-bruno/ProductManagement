using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using FluentAssertions;
using ProductManagement.services;
using ProductManagement.services.decorators;
using ProductManagement.services.caching;
using ProductManagement.dtos;
using ProductManagement.models;

namespace ProductManagement.Tests.Services.Decorators;

public class CachedProductServiceTests
{
    private readonly Mock<IProductService> _mockInnerService;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<ILogger<CachedProductService>> _mockLogger;
    private readonly Mock<IOptions<CacheOptions>> _mockCacheOptions;
    private readonly CachedProductService _cachedProductService;
    private readonly CacheOptions _cacheOptions;

    public CachedProductServiceTests()
    {
        _mockInnerService = new Mock<IProductService>();
        _mockCacheService = new Mock<ICacheService>();
        _mockLogger = new Mock<ILogger<CachedProductService>>();
        _mockCacheOptions = new Mock<IOptions<CacheOptions>>();
        
        _cacheOptions = new CacheOptions
        {
            DefaultTtl = TimeSpan.FromMinutes(5),
            Enabled = true
        };
        
        _mockCacheOptions.Setup(x => x.Value).Returns(_cacheOptions);
        
        _cachedProductService = new CachedProductService(
            _mockInnerService.Object,
            _mockCacheService.Object,
            _mockLogger.Object,
            _mockCacheOptions.Object);
    }

    [Fact]
    public async Task GetByIdAsync_CacheDisabled_CallsInnerServiceDirectly()
    {
        // Arrange
        _cacheOptions.Enabled = false;
        var productId = 1;
        var expectedResult = Result<ProductResponseDto>.Success(
            new ProductResponseDto { Id = productId, Name = "Test Product" });

        _mockInnerService
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _cachedProductService.GetByIdAsync(productId);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
        _mockCacheService.Verify(x => x.GetAsync<ProductResponseDto>(It.IsAny<string>()), Times.Never);
        _mockInnerService.Verify(x => x.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_CacheHit_ReturnsCachedValue()
    {
        // Arrange
        var productId = 1;
        var cachedProduct = new ProductResponseDto { Id = productId, Name = "Cached Product" };
        var cacheKey = ProductCacheKeys.ById(productId);

        _mockCacheService
            .Setup(x => x.GetAsync<ProductResponseDto>(cacheKey))
            .ReturnsAsync(cachedProduct);

        // Act
        var result = await _cachedProductService.GetByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(cachedProduct);
        _mockInnerService.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_CacheMiss_CallsInnerServiceAndCachesResult()
    {
        // Arrange
        var productId = 1;
        var product = new ProductResponseDto { Id = productId, Name = "Test Product" };
        var cacheKey = ProductCacheKeys.ById(productId);
        var innerResult = Result<ProductResponseDto>.Success(product);

        _mockCacheService
            .Setup(x => x.GetAsync<ProductResponseDto>(cacheKey))
            .ReturnsAsync((ProductResponseDto?)null);

        _mockInnerService
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(innerResult);

        // Act
        var result = await _cachedProductService.GetByIdAsync(productId);

        // Assert
        result.Should().BeEquivalentTo(innerResult);
        _mockCacheService.Verify(x => x.SetAsync(cacheKey, product, _cacheOptions.DefaultTtl), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_CacheHit_ReturnsCachedValue()
    {
        // Arrange
        var request = new PaginationRequestDto { Page = 1, PageSize = 10 };
        var cachedResult = new PagedResultDto<ProductSummaryDto>
        {
            Data = new List<ProductSummaryDto> { new() { Id = 1, Name = "Cached Product" } },
            Page = 1,
            PageSize = 10,
            TotalCount = 1
        };
        var cacheKey = ProductCacheKeys.Paged(request);

        _mockCacheService
            .Setup(x => x.GetAsync<PagedResultDto<ProductSummaryDto>>(cacheKey))
            .ReturnsAsync(cachedResult);

        // Act
        var result = await _cachedProductService.GetPagedAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(cachedResult);
        _mockInnerService.Verify(x => x.GetPagedAsync(It.IsAny<PaginationRequestDto>()), Times.Never);
    }

    [Fact]
    public async Task GetPagedAsync_CacheMiss_CallsInnerServiceAndCachesResult()
    {
        // Arrange
        var request = new PaginationRequestDto { Page = 1, PageSize = 10 };
        var pagedResult = new PagedResultDto<ProductSummaryDto>
        {
            Data = new List<ProductSummaryDto> { new() { Id = 1, Name = "Test Product" } },
            Page = 1,
            PageSize = 10,
            TotalCount = 1
        };
        var cacheKey = ProductCacheKeys.Paged(request);
        var innerResult = Result<PagedResultDto<ProductSummaryDto>>.Success(pagedResult);

        _mockCacheService
            .Setup(x => x.GetAsync<PagedResultDto<ProductSummaryDto>>(cacheKey))
            .ReturnsAsync((PagedResultDto<ProductSummaryDto>?)null);

        _mockInnerService
            .Setup(x => x.GetPagedAsync(request))
            .ReturnsAsync(innerResult);

        // Act
        var result = await _cachedProductService.GetPagedAsync(request);

        // Assert
        result.Should().BeEquivalentTo(innerResult);
        _mockCacheService.Verify(x => x.SetAsync(cacheKey, pagedResult, _cacheOptions.DefaultTtl), Times.Once);
        _mockCacheService.Verify(x => x.AddToSetAsync(ProductCacheKeys.PagedIndexSet, cacheKey), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Success_InvalidatesPagedCache()
    {
        // Arrange
        var createRequest = new CreateProductRequestDto { Name = "New Product", Sku = "NEW123", Price = 100m };
        var createdProduct = new ProductResponseDto { Id = 1, Name = "New Product" };
        var innerResult = Result<ProductResponseDto>.Success(createdProduct);
        var pagedCacheKeys = new[] { "product:paged:key1", "product:paged:key2" };

        _mockInnerService
            .Setup(x => x.CreateAsync(createRequest))
            .ReturnsAsync(innerResult);

        _mockCacheService
            .Setup(x => x.GetSetMembersAsync(ProductCacheKeys.PagedIndexSet))
            .ReturnsAsync(pagedCacheKeys);

        // Act
        var result = await _cachedProductService.CreateAsync(createRequest);

        // Assert
        result.Should().BeEquivalentTo(innerResult);
        _mockCacheService.Verify(x => x.GetSetMembersAsync(ProductCacheKeys.PagedIndexSet), Times.Once);
        _mockCacheService.Verify(x => x.RemoveAsync(pagedCacheKeys), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Success_InvalidatesBothSpecificAndPagedCache()
    {
        // Arrange
        var productId = 1;
        var updateRequest = new UpdateProductRequestDto { Name = "Updated Product", Sku = "UPD123", Price = 150m };
        var updatedProduct = new ProductResponseDto { Id = productId, Name = "Updated Product" };
        var innerResult = Result<ProductResponseDto>.Success(updatedProduct);
        var pagedCacheKeys = new[] { "product:paged:key1" };

        _mockInnerService
            .Setup(x => x.UpdateAsync(productId, updateRequest))
            .ReturnsAsync(innerResult);

        _mockCacheService
            .Setup(x => x.GetSetMembersAsync(ProductCacheKeys.PagedIndexSet))
            .ReturnsAsync(pagedCacheKeys);

        // Act
        var result = await _cachedProductService.UpdateAsync(productId, updateRequest);

        // Assert
        result.Should().BeEquivalentTo(innerResult);
        _mockCacheService.Verify(x => x.RemoveAsync(ProductCacheKeys.ById(productId)), Times.Once);
        _mockCacheService.Verify(x => x.GetSetMembersAsync(ProductCacheKeys.PagedIndexSet), Times.Once);
        _mockCacheService.Verify(x => x.RemoveAsync(pagedCacheKeys), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Success_InvalidatesBothSpecificAndPagedCache()
    {
        // Arrange
        var productId = 1;
        var innerResult = Result.Success();
        var pagedCacheKeys = new[] { "product:paged:key1" };

        _mockInnerService
            .Setup(x => x.DeleteAsync(productId))
            .ReturnsAsync(innerResult);

        _mockCacheService
            .Setup(x => x.GetSetMembersAsync(ProductCacheKeys.PagedIndexSet))
            .ReturnsAsync(pagedCacheKeys);

        // Act
        var result = await _cachedProductService.DeleteAsync(productId);

        // Assert
        result.Should().BeEquivalentTo(innerResult);
        _mockCacheService.Verify(x => x.RemoveAsync(ProductCacheKeys.ById(productId)), Times.Once);
        _mockCacheService.Verify(x => x.GetSetMembersAsync(ProductCacheKeys.PagedIndexSet), Times.Once);
        _mockCacheService.Verify(x => x.RemoveAsync(pagedCacheKeys), Times.Once);
    }

    [Fact]
    public async Task SeedAsync_Success_InvalidatesPagedCache()
    {
        // Arrange
        var count = 10;
        var seededProducts = new List<ProductResponseDto>
        {
            new() { Id = 1, Name = "Seeded Product 1" },
            new() { Id = 2, Name = "Seeded Product 2" }
        };
        var innerResult = Result<List<ProductResponseDto>>.Success(seededProducts);
        var pagedCacheKeys = new[] { "product:paged:key1", "product:paged:key2" };

        _mockInnerService
            .Setup(x => x.SeedAsync(count))
            .ReturnsAsync(innerResult);

        _mockCacheService
            .Setup(x => x.GetSetMembersAsync(ProductCacheKeys.PagedIndexSet))
            .ReturnsAsync(pagedCacheKeys);

        // Act
        var result = await _cachedProductService.SeedAsync(count);

        // Assert
        result.Should().BeEquivalentTo(innerResult);
        _mockCacheService.Verify(x => x.GetSetMembersAsync(ProductCacheKeys.PagedIndexSet), Times.Once);
        _mockCacheService.Verify(x => x.RemoveAsync(pagedCacheKeys), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_CacheDisabled_DoesNotInvalidateCache()
    {
        // Arrange
        _cacheOptions.Enabled = false;
        var createRequest = new CreateProductRequestDto { Name = "New Product", Sku = "NEW123", Price = 100m };
        var createdProduct = new ProductResponseDto { Id = 1, Name = "New Product" };
        var innerResult = Result<ProductResponseDto>.Success(createdProduct);

        _mockInnerService
            .Setup(x => x.CreateAsync(createRequest))
            .ReturnsAsync(innerResult);

        // Act
        var result = await _cachedProductService.CreateAsync(createRequest);

        // Assert
        result.Should().BeEquivalentTo(innerResult);
        _mockCacheService.Verify(x => x.GetSetMembersAsync(It.IsAny<string>()), Times.Never);
        _mockCacheService.Verify(x => x.RemoveAsync(It.IsAny<string[]>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_InnerServiceFailure_DoesNotCache()
    {
        // Arrange
        var productId = 1;
        var cacheKey = ProductCacheKeys.ById(productId);
        var failureResult = Result<ProductResponseDto>.Failure("Product not found");

        _mockCacheService
            .Setup(x => x.GetAsync<ProductResponseDto>(cacheKey))
            .ReturnsAsync((ProductResponseDto?)null);

        _mockInnerService
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _cachedProductService.GetByIdAsync(productId);

        // Assert
        result.Should().BeEquivalentTo(failureResult);
        _mockCacheService.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<ProductResponseDto>(), It.IsAny<TimeSpan>()), Times.Never);
    }
}