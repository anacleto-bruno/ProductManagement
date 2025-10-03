using FluentAssertions;
using ProductManagement.helpers;
using ProductManagement.entities;
using Microsoft.EntityFrameworkCore;
using ProductManagement.infrastructure;

namespace ProductManagement.Tests.Helpers;

/// <summary>
/// Tests for ProductQueryExtensions methods - these use InMemory database for realistic testing
/// </summary>
public class ProductQueryExtensionsTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly IQueryable<Product> _testQuery;

    public ProductQueryExtensionsTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        SeedTestData();
        _testQuery = _context.Products;
    }

    private void SeedTestData()
    {
        var products = new List<Product>
        {
            new()
            {
                Id = 1,
                Name = "Apple iPhone",
                Description = "Latest smartphone from Apple",
                Brand = "Apple",
                Model = "iPhone 14",
                Sku = "APL-IPH-14",
                Price = 999.99m,
                Category = "Electronics",
                CreatedAt = new DateTime(2023, 1, 1)
            },
            new()
            {
                Id = 2,
                Name = "Samsung Galaxy",
                Description = "Android smartphone",
                Brand = "Samsung",
                Model = "Galaxy S23",
                Sku = "SAM-GAL-S23",
                Price = 799.99m,
                Category = "Electronics",
                CreatedAt = new DateTime(2023, 2, 1)
            },
            new()
            {
                Id = 3,
                Name = "Nike Air Max",
                Description = "Running shoes",
                Brand = "Nike",
                Model = "Air Max 90",
                Sku = "NIKE-AM-90",
                Price = 120.00m,
                Category = "Footwear",
                CreatedAt = new DateTime(2023, 3, 1)
            },
            new()
            {
                Id = 4,
                Name = "Adidas Ultraboost",
                Description = "Premium running shoes",
                Brand = "Adidas",
                Model = "Ultraboost 22",
                Sku = "ADI-UB-22",
                Price = 180.00m,
                Category = "Footwear",
                CreatedAt = new DateTime(2023, 4, 1)
            },
            new()
            {
                Id = 5,
                Name = "Dell Laptop",
                Description = "Business laptop",
                Brand = "Dell",
                Model = "XPS 13",
                Sku = "DELL-XPS-13",
                Price = 1299.99m,
                Category = "Electronics",
                CreatedAt = new DateTime(2023, 5, 1)
            }
        };

        _context.Products.AddRange(products);
        _context.SaveChanges();
    }

    [Fact]
    public void WhereCategory_ValidCategory_FiltersCorrectly()
    {
        // Act
        var result = _testQuery.WhereCategory("Electronics").ToList();

        // Assert
        result.Should().HaveCount(3);
        result.Should().OnlyContain(p => p.Category == "Electronics");
        result.Select(p => p.Name).Should().Contain(new[] { "Apple iPhone", "Samsung Galaxy", "Dell Laptop" });
    }

    [Fact]
    public void WhereCategory_CaseInsensitive_FiltersCorrectly()
    {
        // Act
        var result = _testQuery.WhereCategory("electronics").ToList();

        // Assert
        result.Should().HaveCount(3);
        result.Should().OnlyContain(p => p.Category == "Electronics");
    }

    [Fact]
    public void WhereCategory_NullCategory_ReturnsAllProducts()
    {
        // Act
        var result = _testQuery.WhereCategory(null).ToList();

        // Assert
        result.Should().HaveCount(5);
    }

    [Fact]
    public void WhereCategory_EmptyCategory_ReturnsAllProducts()
    {
        // Act
        var result = _testQuery.WhereCategory("").ToList();

        // Assert
        result.Should().HaveCount(5);
    }

    [Fact]
    public void WhereCategory_NonExistentCategory_ReturnsEmpty()
    {
        // Act
        var result = _testQuery.WhereCategory("NonExistent").ToList();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void WherePriceRange_MinPriceOnly_FiltersCorrectly()
    {
        // Act
        var result = _testQuery.WherePriceRange(800m, null).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.Price >= 800m);
        result.Select(p => p.Name).Should().Contain(new[] { "Apple iPhone", "Dell Laptop" });
    }

    [Fact]
    public void WherePriceRange_MaxPriceOnly_FiltersCorrectly()
    {
        // Act
        var result = _testQuery.WherePriceRange(null, 200m).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.Price <= 200m);
        result.Select(p => p.Name).Should().Contain(new[] { "Nike Air Max", "Adidas Ultraboost" });
    }

    [Fact]
    public void WherePriceRange_BothMinAndMax_FiltersCorrectly()
    {
        // Act
        var result = _testQuery.WherePriceRange(100m, 900m).ToList();

        // Assert
        result.Should().HaveCount(3);
        result.Should().OnlyContain(p => p.Price >= 100m && p.Price <= 900m);
        result.Select(p => p.Name).Should().Contain(new[] { "Samsung Galaxy", "Nike Air Max", "Adidas Ultraboost" });
    }

    [Fact]
    public void WherePriceRange_NoFilters_ReturnsAllProducts()
    {
        // Act
        var result = _testQuery.WherePriceRange(null, null).ToList();

        // Assert
        result.Should().HaveCount(5);
    }

    [Fact]
    public void WhereSearch_ByName_FiltersCorrectly()
    {
        // Act
        var result = _testQuery.WhereSearch("iPhone").ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Apple iPhone");
    }

    [Fact]
    public void WhereSearch_ByDescription_FiltersCorrectly()
    {
        // Act
        var result = _testQuery.WhereSearch("smartphone").ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Select(p => p.Name).Should().Contain(new[] { "Apple iPhone", "Samsung Galaxy" });
    }

    [Fact]
    public void WhereSearch_ByBrand_FiltersCorrectly()
    {
        // Act
        var result = _testQuery.WhereSearch("Nike").ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Nike Air Max");
    }

    [Fact]
    public void WhereSearch_ByModel_FiltersCorrectly()
    {
        // Act
        var result = _testQuery.WhereSearch("XPS").ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Dell Laptop");
    }

    [Fact]
    public void WhereSearch_BySku_FiltersCorrectly()
    {
        // Act
        var result = _testQuery.WhereSearch("SAM-GAL").ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Samsung Galaxy");
    }

    [Fact]
    public void WhereSearch_CaseInsensitive_FiltersCorrectly()
    {
        // Act
        var result = _testQuery.WhereSearch("APPLE").ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Apple iPhone");
    }

    [Fact]
    public void WhereSearch_PartialMatch_FiltersCorrectly()
    {
        // Act
        var result = _testQuery.WhereSearch("run").ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Select(p => p.Name).Should().Contain(new[] { "Nike Air Max", "Adidas Ultraboost" });
    }

    [Fact]
    public void WhereSearch_NullSearchTerm_ReturnsAllProducts()
    {
        // Act
        var result = _testQuery.WhereSearch(null).ToList();

        // Assert
        result.Should().HaveCount(5);
    }

    [Fact]
    public void WhereSearch_EmptySearchTerm_ReturnsAllProducts()
    {
        // Act
        var result = _testQuery.WhereSearch("").ToList();

        // Assert
        result.Should().HaveCount(5);
    }

    [Fact]
    public void OrderByField_ByName_Ascending_SortsCorrectly()
    {
        // Act
        var result = _testQuery.OrderByField("name", false).ToList();

        // Assert
        result.Should().HaveCount(5);
        result[0].Name.Should().Be("Adidas Ultraboost");
        result[1].Name.Should().Be("Apple iPhone");
        result[2].Name.Should().Be("Dell Laptop");
        result[3].Name.Should().Be("Nike Air Max");
        result[4].Name.Should().Be("Samsung Galaxy");
    }

    [Fact]
    public void OrderByField_ByName_Descending_SortsCorrectly()
    {
        // Act
        var result = _testQuery.OrderByField("name", true).ToList();

        // Assert
        result.Should().HaveCount(5);
        result[0].Name.Should().Be("Samsung Galaxy");
        result[1].Name.Should().Be("Nike Air Max");
        result[2].Name.Should().Be("Dell Laptop");
        result[3].Name.Should().Be("Apple iPhone");
        result[4].Name.Should().Be("Adidas Ultraboost");
    }

    [Fact]
    public void OrderByField_ByPrice_Ascending_SortsCorrectly()
    {
        // Act
        var result = _testQuery.OrderByField("price", false).ToList();

        // Assert
        result.Should().HaveCount(5);
        result[0].Price.Should().Be(120.00m);
        result[1].Price.Should().Be(180.00m);
        result[2].Price.Should().Be(799.99m);
        result[3].Price.Should().Be(999.99m);
        result[4].Price.Should().Be(1299.99m);
    }

    [Fact]
    public void OrderByField_ByPrice_Descending_SortsCorrectly()
    {
        // Act
        var result = _testQuery.OrderByField("price", true).ToList();

        // Assert
        result.Should().HaveCount(5);
        result[0].Price.Should().Be(1299.99m);
        result[1].Price.Should().Be(999.99m);
        result[2].Price.Should().Be(799.99m);
        result[3].Price.Should().Be(180.00m);
        result[4].Price.Should().Be(120.00m);
    }

    [Fact]
    public void OrderByField_ByBrand_CaseInsensitive_SortsCorrectly()
    {
        // Act
        var result = _testQuery.OrderByField("BRAND", false).ToList();

        // Assert
        result.Should().HaveCount(5);
        result[0].Brand.Should().Be("Adidas");
        result[1].Brand.Should().Be("Apple");
        result[2].Brand.Should().Be("Dell");
        result[3].Brand.Should().Be("Nike");
        result[4].Brand.Should().Be("Samsung");
    }

    [Fact]
    public void OrderByField_ByCreatedAt_Ascending_SortsCorrectly()
    {
        // Act
        var result = _testQuery.OrderByField("createdat", false).ToList();

        // Assert
        result.Should().HaveCount(5);
        result[0].CreatedAt.Should().Be(new DateTime(2023, 1, 1));
        result[1].CreatedAt.Should().Be(new DateTime(2023, 2, 1));
        result[2].CreatedAt.Should().Be(new DateTime(2023, 3, 1));
        result[3].CreatedAt.Should().Be(new DateTime(2023, 4, 1));
        result[4].CreatedAt.Should().Be(new DateTime(2023, 5, 1));
    }

    [Fact]
    public void OrderByField_UnknownField_DefaultsToCreatedAt()
    {
        // Act
        var result = _testQuery.OrderByField("unknownfield", false).ToList();

        // Assert
        result.Should().HaveCount(5);
        result[0].CreatedAt.Should().Be(new DateTime(2023, 1, 1));
        result[4].CreatedAt.Should().Be(new DateTime(2023, 5, 1));
    }

    [Fact]
    public void OrderByField_NullSortBy_DefaultsToCreatedAt()
    {
        // Act
        var result = _testQuery.OrderByField(null, false).ToList();

        // Assert
        result.Should().HaveCount(5);
        result[0].CreatedAt.Should().Be(new DateTime(2023, 1, 1));
        result[4].CreatedAt.Should().Be(new DateTime(2023, 5, 1));
    }

    [Fact]
    public void ChainedExtensions_MultipleFilters_WorkTogether()
    {
        // Act
        var result = _testQuery
            .WhereCategory("Electronics")
            .WherePriceRange(800m, null)
            .WhereSearch("phone")
            .OrderByField("price", true)
            .ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Apple iPhone"); // Only iPhone matches Electronics + price >= 800 + contains "phone"
        result.Should().OnlyContain(p => p.Category == "Electronics");
        result.Should().OnlyContain(p => p.Price >= 800m);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}