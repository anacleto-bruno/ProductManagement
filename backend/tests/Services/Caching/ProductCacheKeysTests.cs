using FluentAssertions;
using ProductManagement.services.caching;
using ProductManagement.dtos;

namespace ProductManagement.Tests.Services.Caching;

public class ProductCacheKeysTests
{
    [Fact]
    public void ById_GeneratesCorrectKey()
    {
        // Arrange
        var productId = 123;

        // Act
        var key = ProductCacheKeys.ById(productId);

        // Assert
        key.Should().Be("product:by-id:123");
    }

    [Fact]
    public void Paged_SimpleRequest_GeneratesConsistentKey()
    {
        // Arrange
        var request = new PaginationRequestDto
        {
            Page = 1,
            PageSize = 20,
            SearchTerm = null,
            Category = null,
            MinPrice = null,
            MaxPrice = null,
            SortBy = null,
            Descending = false
        };

        // Act
        var key1 = ProductCacheKeys.Paged(request);
        var key2 = ProductCacheKeys.Paged(request);

        // Assert
        key1.Should().Be(key2);
        key1.Should().StartWith("product:paged:");
        key1.Length.Should().BeGreaterThan(20); // Should contain hash
    }

    [Fact]
    public void Paged_DifferentRequests_GenerateDifferentKeys()
    {
        // Arrange
        var request1 = new PaginationRequestDto
        {
            Page = 1,
            PageSize = 20,
            SearchTerm = "test"
        };

        var request2 = new PaginationRequestDto
        {
            Page = 2,
            PageSize = 20,
            SearchTerm = "test"
        };

        var request3 = new PaginationRequestDto
        {
            Page = 1,
            PageSize = 20,
            SearchTerm = "different"
        };

        // Act
        var key1 = ProductCacheKeys.Paged(request1);
        var key2 = ProductCacheKeys.Paged(request2);
        var key3 = ProductCacheKeys.Paged(request3);

        // Assert
        key1.Should().NotBe(key2);
        key1.Should().NotBe(key3);
        key2.Should().NotBe(key3);
    }

    [Fact]
    public void Paged_ComplexRequest_GeneratesConsistentKey()
    {
        // Arrange
        var request = new PaginationRequestDto
        {
            Page = 2,
            PageSize = 50,
            SearchTerm = "laptop computer",
            Category = "Electronics",
            MinPrice = 100.50m,
            MaxPrice = 2000.00m,
            SortBy = "price",
            Descending = true
        };

        // Act
        var key1 = ProductCacheKeys.Paged(request);
        var key2 = ProductCacheKeys.Paged(request);

        // Assert
        key1.Should().Be(key2);
        key1.Should().StartWith("product:paged:");
    }

    [Fact]
    public void Paged_NullValues_HandledProperly()
    {
        // Arrange
        var request = new PaginationRequestDto
        {
            Page = 1,
            PageSize = 10,
            SearchTerm = null,
            Category = null,
            MinPrice = null,
            MaxPrice = null,
            SortBy = null,
            Descending = false
        };

        // Act & Assert (should not throw)
        var key = ProductCacheKeys.Paged(request);
        
        key.Should().NotBeNullOrEmpty();
        key.Should().StartWith("product:paged:");
    }

    [Fact]
    public void PagedIndexSet_HasCorrectConstantValue()
    {
        // Assert
        ProductCacheKeys.PagedIndexSet.Should().Be("product:paged:index");
    }
}