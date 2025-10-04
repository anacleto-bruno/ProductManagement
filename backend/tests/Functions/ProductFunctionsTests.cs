using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ProductManagement.dtos;
using ProductManagement.functions;
using ProductManagement.models;
using ProductManagement.services;
using ProductManagement.helpers;
using ProductManagement.UnitTests.TestHelpers;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace ProductManagement.UnitTests.Functions;

public class ProductFunctionsTests
{
    private readonly Mock<ILogger<ProductFunctions>> _loggerMock = new();
    private readonly Mock<IProductService> _serviceMock = new();
    private readonly ProductFunctions _functions;
    private readonly TestFunctionContext _context = new();

    public ProductFunctionsTests()
    {
        _functions = new ProductFunctions(_loggerMock.Object, _serviceMock.Object);
    }

    private static ProductResponseDto CreateProduct(int id = 1) => new()
    {
        Id = id,
        Name = $"Product {id}",
        Model = "M1",
        Brand = "BrandX",
        Sku = $"SKU{id}",
        Price = 9.99m,
        Category = "Cat"
    };

    #region GetByIdAsync
    [Fact]
    public async Task GetByIdAsync_IdLessOrEqualZero_ReturnsBadRequest()
    {
        var request = new TestHttpRequestData(_context);

        var response = await _functions.GetByIdAsync(request, 0);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await ReadBodyAsync(response);
        body.Should().Contain("Product ID must be greater than 0");
        _serviceMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_ProductFound_ReturnsOkWithProduct()
    {
        var request = new TestHttpRequestData(_context);
        var product = CreateProduct();
        _serviceMock.Setup(s => s.GetByIdAsync(product.Id)).ReturnsAsync(Result<ProductResponseDto>.Success(product));

        var response = await _functions.GetByIdAsync(request, product.Id);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await ReadBodyAsync(response);
        body.Should().Contain(product.Name);
        _serviceMock.Verify(s => s.GetByIdAsync(product.Id), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ReturnsNotFound()
    {
        var request = new TestHttpRequestData(_context);
        _serviceMock.Setup(s => s.GetByIdAsync(42)).ReturnsAsync(Result<ProductResponseDto>.Failure("Product not found"));

        var response = await _functions.GetByIdAsync(request, 42);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var body = await ReadBodyAsync(response);
        body.Should().Contain("Product not found");
    }
    #endregion

    #region GetPagedAsync
    [Fact]
    public async Task GetPagedAsync_ReturnsPagedResult()
    {
        var request = new TestHttpRequestData(_context, url: "https://localhost/products?page=2&pageSize=5&searchTerm=test&descending=true");
        var paged = new PagedResultDto<ProductSummaryDto>
        {
            Page = 2,
            PageSize = 5,
            TotalCount = 12,
            TotalPages = 3,
            Data = new List<ProductSummaryDto>{ new(){ Id=1, Name="P1", Model="M", Brand="B", Sku="S", Price=1m } }
        };
        _serviceMock.Setup(s => s.GetPagedAsync(It.Is<PaginationRequestDto>(p => p.Page==2 && p.PageSize==5 && p.SearchTerm=="test" && p.Descending))).ReturnsAsync(Result<PagedResultDto<ProductSummaryDto>>.Success(paged));

        var response = await _functions.GetPagedAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await ReadBodyAsync(response);
        body.Should().Contain("\"page\":2");
        _serviceMock.VerifyAll();
    }
    #endregion

    #region CreateAsync
    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsCreated()
    {
        var create = new CreateProductRequestDto
        {
            Name = "New Product",
            Model = "M1",
            Brand = "B1",
            Sku = "SKU1",
            Price = 10,
            Category = "Cat"
        };
        var json = System.Text.Json.JsonSerializer.Serialize(create);
        var request = new TestHttpRequestData(_context, method: "POST", body: json, url: "https://localhost/products");
        var product = CreateProduct(10);
        _serviceMock.Setup(s => s.CreateAsync(It.Is<CreateProductRequestDto>(c => c.Name==create.Name))).ReturnsAsync(Result<ProductResponseDto>.Success(product));

        var response = await _functions.CreateAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await ReadBodyAsync(response);
        body.Should().Contain("\"id\":10");
    }

    [Fact]
    public async Task CreateAsync_ServiceFailure_ReturnsBadRequest()
    {
        var create = new CreateProductRequestDto { Name = "Bad Product" };
        var json = System.Text.Json.JsonSerializer.Serialize(create);
        var request = new TestHttpRequestData(_context, method: "POST", body: json);
        _serviceMock.Setup(s => s.CreateAsync(It.IsAny<CreateProductRequestDto>()))
            .ReturnsAsync(Result<ProductResponseDto>.Failure("validation failed: name"));

        var response = await _functions.CreateAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await ReadBodyAsync(response)).Should().Contain("validation failed");
    }
    #endregion

    #region UpdateAsync
    [Fact]
    public async Task UpdateAsync_InvalidId_ReturnsBadRequest()
    {
        var request = new TestHttpRequestData(_context, method: "PUT");

        var response = await _functions.UpdateAsync(request, 0);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await ReadBodyAsync(response)).Should().Contain("greater than 0");
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_ReturnsOk()
    {
        var update = new UpdateProductRequestDto
        {
            Name="Updated", Model="M", Brand="B", Sku="S", Price=20
        };
        var json = System.Text.Json.JsonSerializer.Serialize(update);
        var request = new TestHttpRequestData(_context, method: "PUT", body: json);
        var product = CreateProduct(5) with { Name = "Updated" };
        _serviceMock.Setup(s => s.UpdateAsync(5, It.IsAny<UpdateProductRequestDto>())).ReturnsAsync(Result<ProductResponseDto>.Success(product));

        var response = await _functions.UpdateAsync(request, 5);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await ReadBodyAsync(response)).Should().Contain("Updated");
    }
    #endregion

    #region DeleteAsync
    [Fact]
    public async Task DeleteAsync_InvalidId_ReturnsBadRequest()
    {
        var request = new TestHttpRequestData(_context, method: "DELETE");
        var response = await _functions.DeleteAsync(request, -1);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteAsync_Success_ReturnsOk()
    {
        var request = new TestHttpRequestData(_context, method: "DELETE");
        _serviceMock.Setup(s => s.DeleteAsync(3)).ReturnsAsync(Result.Success());

        var response = await _functions.DeleteAsync(request, 3);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    #endregion

    #region SeedAsync
    [Fact]
    public async Task SeedAsync_InvalidCount_ReturnsBadRequest()
    {
        var request = new TestHttpRequestData(_context, method: "POST");
        var response = await _functions.SeedAsync(request, 0);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SeedAsync_Success_ReturnsOkWithCount()
    {
        var request = new TestHttpRequestData(_context, method: "POST");
        var products = new List<ProductResponseDto> { CreateProduct(1), CreateProduct(2) };
        _serviceMock.Setup(s => s.SeedAsync(2)).ReturnsAsync(Result<List<ProductResponseDto>>.Success(products));

        var response = await _functions.SeedAsync(request, 2);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await ReadBodyAsync(response)).Should().Contain("\"count\":2");
    }

    [Fact]
    public async Task SeedAsync_Failure_ReturnsBadRequest()
    {
        var request = new TestHttpRequestData(_context, method: "POST");
        _serviceMock.Setup(s => s.SeedAsync(5)).ReturnsAsync(Result<List<ProductResponseDto>>.Failure("seeding failed"));

        var response = await _functions.SeedAsync(request, 5);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await ReadBodyAsync(response)).Should().Contain("seeding failed");
    }
    #endregion

    private static async Task<string> ReadBodyAsync(HttpResponseData response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(response.Body, leaveOpen: true);
        return await reader.ReadToEndAsync();
    }
}
