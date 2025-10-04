using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductManagement.dtos;
using ProductManagement.entities;
using ProductManagement.infrastructure;
using ProductManagement.infrastructure.repositories;
using Xunit;

namespace ProductManagement.Tests.Infrastructure.Repositories;

public class ProductRepositoryTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"product-repo-tests-{Guid.NewGuid()}")
            .Options;
        return new ApplicationDbContext(options);
    }

    private static Product BuildProduct(int id, string name, string model, string brand, string sku, decimal price, string? category = null)
        => new()
        {
            Id = id,
            Name = name,
            Model = model,
            Brand = brand,
            Sku = sku,
            Price = price,
            Category = category,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

    private static void SeedProducts(ApplicationDbContext ctx)
    {
        var colors = new List<Color>
        {
            new() { Id = 1, Name = "Red",   HexCode = "#FF0000" },
            new() { Id = 2, Name = "Green", HexCode = "#00FF00" }
        };
        var sizes = new List<Size>
        {
            new() { Id = 1, Name = "Small",  Code = "S", SortOrder = 1 },
            new() { Id = 2, Name = "Medium", Code = "M", SortOrder = 2 }
        };
        ctx.Colors.AddRange(colors);
        ctx.Sizes.AddRange(sizes);

        var products = new List<Product>
        {
            BuildProduct(1, "Alpha Shoe",   "M1", "BrandA", "SKU-A", 50m, "Footwear"),
            BuildProduct(2, "Beta Jacket",  "M2", "BrandB", "SKU-B", 120m, "Apparel"),
            BuildProduct(3, "Gamma Shoe",   "M3", "BrandA", "SKU-C", 80m, "Footwear"),
            BuildProduct(4, "Delta Pants",  "M4", "BrandC", "SKU-D", 60m, "Apparel"),
            BuildProduct(5, "Epsilon Shoe", "M5", "BrandB", "SKU-E", 200m, "Footwear")
        };

        ctx.Products.AddRange(products);

        // relationships
        ctx.ProductColors.AddRange(
            new ProductColor { ProductId = 1, ColorId = 1 },
            new ProductColor { ProductId = 1, ColorId = 2 },
            new ProductColor { ProductId = 3, ColorId = 1 }
        );
        ctx.ProductSizes.AddRange(
            new ProductSize { ProductId = 1, SizeId = 1 },
            new ProductSize { ProductId = 1, SizeId = 2 },
            new ProductSize { ProductId = 2, SizeId = 2 }
        );
        ctx.SaveChanges();
    }

    [Fact]
    public async Task GetProductDtoByIdAsync_WhenExists_ReturnsPopulatedDto()
    {
        using var ctx = CreateContext();
        SeedProducts(ctx);
        var repo = new ProductRepository(ctx);

        var dto = await repo.GetProductDtoByIdAsync(1);

        dto.Should().NotBeNull();
        dto!.Id.Should().Be(1);
        dto.Colors.Should().HaveCount(2);
        dto.Sizes.Should().HaveCount(2);
        dto.Name.Should().Be("Alpha Shoe");
    }

    [Fact]
    public async Task GetProductDtoByIdAsync_WhenNotFound_ReturnsNull()
    {
        using var ctx = CreateContext();
        SeedProducts(ctx);
        var repo = new ProductRepository(ctx);

        var dto = await repo.GetProductDtoByIdAsync(999);

        dto.Should().BeNull();
    }

    [Fact]
    public async Task GetProductDtosAsync_FiltersSortsAndPagesCorrectly()
    {
        using var ctx = CreateContext();
        SeedProducts(ctx);
        var repo = new ProductRepository(ctx);
        var request = new PaginationRequestDto
        {
            Page = 1,
            PageSize = 2,
            Category = "Footwear",
            SortBy = "price",
            Descending = true
        };

        var page = await repo.GetProductDtosAsync(request);

        page.TotalCount.Should().Be(3); // 3 footwear items
        page.Data.Should().HaveCount(2); // first page size 2
        page.Data.First().Price.Should().BeGreaterThan(page.Data.Last().Price); // sorted desc by price
        page.Data.All(p => p.Category == "Footwear").Should().BeTrue();
    }

    [Fact]
    public async Task GetProductDtosAsync_SearchAndPriceRangeFiltersApply()
    {
        using var ctx = CreateContext();
        SeedProducts(ctx);
        var repo = new ProductRepository(ctx);
        var request = new PaginationRequestDto
        {
            Page = 1,
            PageSize = 10,
            SearchTerm = "shoe", // matches Alpha, Gamma, Epsilon
            MinPrice = 60m,
            MaxPrice = 150m
        };

        var page = await repo.GetProductDtosAsync(request);

        page.Data.Select(p => p.Name).Should().BeEquivalentTo(new[] { "Gamma Shoe" });
        page.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task ExistsBySkuAsync_ReturnsTrueWhenExistsFalseOtherwiseAndHonorsExcludeId()
    {
        using var ctx = CreateContext();
        SeedProducts(ctx);
        var repo = new ProductRepository(ctx);

        (await repo.ExistsBySkuAsync("SKU-A")).Should().BeTrue();
        (await repo.ExistsBySkuAsync("SKU-NOPE")).Should().BeFalse();

        // Exclude existing id -> should return false (as if SKU is unique for update scenario)
        var product = ctx.Products.First(p => p.Sku == "SKU-A");
        (await repo.ExistsBySkuAsync("SKU-A", product.Id)).Should().BeFalse();
    }

    [Fact]
    public async Task GetColorsAsync_ReturnsAlphabetical()
    {
        using var ctx = CreateContext();
        SeedProducts(ctx);
        var repo = new ProductRepository(ctx);

        var colors = await repo.GetColorsAsync();
        colors.Select(c => c.Name).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task GetSizesAsync_ReturnsOrderedBySortOrderThenName()
    {
        using var ctx = CreateContext();
        SeedProducts(ctx);
        // Add another size with null SortOrder and name earlier alphabetically to test ordering
    // Provide explicit SortOrder to satisfy required property constraints in EF configuration.
    ctx.Sizes.Add(new Size { Id = 3, Name = "Extra", Code = "X", SortOrder = 99 });
        ctx.SaveChanges();
        var repo = new ProductRepository(ctx);

        var sizes = await repo.GetSizesAsync();
    sizes.Select(s => s.Id).Should().ContainInOrder(1,2,3); // SortOrder 1,2 then 99
    }

    [Fact]
    public async Task GetByIdAsync_IncludesRelatedCollections()
    {
        using var ctx = CreateContext();
        SeedProducts(ctx);
        var repo = new ProductRepository(ctx);

        var product = await repo.GetByIdAsync(1);

        product.Should().NotBeNull();
        product!.ProductColors.Should().HaveCount(2);
        product.ProductSizes.Should().HaveCount(2);
    }
}
