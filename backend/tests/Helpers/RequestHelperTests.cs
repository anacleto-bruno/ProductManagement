using FluentAssertions;
using ProductManagement.helpers;
using ProductManagement.dtos;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Moq;
using System.Text;
using System.Text.Json;

namespace ProductManagement.Tests.Helpers;

/// <summary>
/// Tests for RequestHelper static methods
/// </summary>
public class RequestHelperTests
{
    [Fact]
    public async Task ParseJsonBodyAsync_ValidJson_DeserializesCorrectly()
    {
        // Arrange
        var createRequest = new CreateProductRequestDto
        {
            Name = "Test Product",
            Description = "Test Description",
            Model = "TestModel",
            Brand = "TestBrand",
            Sku = "TEST-001",
            Price = 99.99m,
            Category = "Electronics",
            ColorIds = new List<int> { 1, 2 },
            SizeIds = new List<int> { 1 }
        };

        var json = JsonSerializer.Serialize(createRequest, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var mockRequest = CreateMockHttpRequestData(json);

        // Act
        var result = await RequestHelper.ParseJsonBodyAsync<CreateProductRequestDto>(mockRequest.Object);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test Product");
        result.Description.Should().Be("Test Description");
        result.Model.Should().Be("TestModel");
        result.Brand.Should().Be("TestBrand");
        result.Sku.Should().Be("TEST-001");
        result.Price.Should().Be(99.99m);
        result.Category.Should().Be("Electronics");
        result.ColorIds.Should().BeEquivalentTo(new List<int> { 1, 2 });
        result.SizeIds.Should().BeEquivalentTo(new List<int> { 1 });
    }

    [Fact]
    public async Task ParseJsonBodyAsync_EmptyBody_ReturnsNewInstance()
    {
        // Arrange
        var mockRequest = CreateMockHttpRequestData("");

        // Act
        var result = await RequestHelper.ParseJsonBodyAsync<CreateProductRequestDto>(mockRequest.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CreateProductRequestDto>();
        // Empty string body results in default values for the new object
        result.Name.Should().Be(""); // Default string value from deserialization
        result.ColorIds.Should().BeEmpty(); // Deserialization creates empty lists
        result.SizeIds.Should().BeEmpty();
    }

    [Fact]
    public async Task ParseJsonBodyAsync_NullBody_ReturnsNewInstance()
    {
        // Arrange
        var mockRequest = CreateMockHttpRequestData(null);

        // Act
        var result = await RequestHelper.ParseJsonBodyAsync<CreateProductRequestDto>(mockRequest.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CreateProductRequestDto>();
    }

    [Fact]
    public async Task ParseJsonBodyAsync_InvalidJson_ThrowsArgumentException()
    {
        // Arrange
        var invalidJson = "{ invalid json content }";
        var mockRequest = CreateMockHttpRequestData(invalidJson);

        // Act & Assert
        await FluentActions
            .Invoking(async () => await RequestHelper.ParseJsonBodyAsync<CreateProductRequestDto>(mockRequest.Object))
            .Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("Invalid JSON format");
    }

    [Fact]
    public async Task ParseJsonBodyAsync_CaseInsensitiveDeserialization_WorksCorrectly()
    {
        // Arrange
        var json = @"{
            ""NAME"": ""Test Product"",
            ""description"": ""Test Description"",
            ""PRICE"": 99.99,
            ""colorIds"": [1, 2]
        }";

        var mockRequest = CreateMockHttpRequestData(json);

        // Act
        var result = await RequestHelper.ParseJsonBodyAsync<CreateProductRequestDto>(mockRequest.Object);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test Product");
        result.Description.Should().Be("Test Description");
        result.Price.Should().Be(99.99m);
        result.ColorIds.Should().BeEquivalentTo(new List<int> { 1, 2 });
    }

    [Fact]
    public void ParseQueryString_ValidQueryString_ParsesCorrectly()
    {
        // Arrange
        var queryString = "page=1&pageSize=10&searchTerm=test&category=electronics&minPrice=10.00&maxPrice=500.00";

        // Act
        var result = RequestHelper.ParseQueryString(queryString);

        // Assert
        result.Should().HaveCount(6);
        result["page"].Should().Be("1");
        result["pageSize"].Should().Be("10");
        result["searchTerm"].Should().Be("test");
        result["category"].Should().Be("electronics");
        result["minPrice"].Should().Be("10.00");
        result["maxPrice"].Should().Be("500.00");
    }

    [Fact]
    public void ParseQueryString_QueryStringWithQuestionMark_ParsesCorrectly()
    {
        // Arrange
        var queryString = "?page=1&pageSize=10&searchTerm=test";

        // Act
        var result = RequestHelper.ParseQueryString(queryString);

        // Assert
        result.Should().HaveCount(3);
        result["page"].Should().Be("1");
        result["pageSize"].Should().Be("10");
        result["searchTerm"].Should().Be("test");
    }

    [Fact]
    public void ParseQueryString_EmptyQueryString_ReturnsEmptyDictionary()
    {
        // Arrange
        var queryString = "";

        // Act
        var result = RequestHelper.ParseQueryString(queryString);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ParseQueryString_NullQueryString_ReturnsEmptyDictionary()
    {
        // Arrange
        string? queryString = null;

        // Act
        var result = RequestHelper.ParseQueryString(queryString!);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ParseQueryString_QueryStringWithUrlEncoding_DecodesCorrectly()
    {
        // Arrange
        var queryString = "searchTerm=hello%20world&category=home%26garden&special=%21%40%23%24";

        // Act
        var result = RequestHelper.ParseQueryString(queryString);

        // Assert
        result.Should().HaveCount(3);
        result["searchTerm"].Should().Be("hello world");
        result["category"].Should().Be("home&garden");
        result["special"].Should().Be("!@#$");
    }

    [Fact]
    public void ParseQueryString_QueryStringWithEmptyValues_HandlesCorrectly()
    {
        // Arrange
        var queryString = "page=1&searchTerm=&category=electronics&emptyParam=";

        // Act
        var result = RequestHelper.ParseQueryString(queryString);

        // Assert
        result.Should().HaveCount(4);
        result["page"].Should().Be("1");
        result["searchTerm"].Should().Be("");
        result["category"].Should().Be("electronics");
        result["emptyParam"].Should().Be("");
    }

    [Fact]
    public void ParseQueryString_QueryStringWithInvalidPairs_IgnoresInvalidPairs()
    {
        // Arrange
        var queryString = "page=1&invalidpair&category=electronics&anotherbadone&price=100";

        // Act
        var result = RequestHelper.ParseQueryString(queryString);

        // Assert
        result.Should().HaveCount(3);
        result["page"].Should().Be("1");
        result["category"].Should().Be("electronics");
        result["price"].Should().Be("100");
        result.Should().NotContainKey("invalidpair");
        result.Should().NotContainKey("anotherbadone");
    }

    [Fact]
    public void ParseQueryString_QueryStringWithDuplicateKeys_UsesLastValue()
    {
        // Arrange
        var queryString = "page=1&category=electronics&page=2&category=clothing";

        // Act
        var result = RequestHelper.ParseQueryString(queryString);

        // Assert
        result.Should().HaveCount(2);
        result["page"].Should().Be("2"); // Last value wins
        result["category"].Should().Be("clothing"); // Last value wins
    }

    private static Mock<HttpRequestData> CreateMockHttpRequestData(string? body)
    {
        var mockRequest = new Mock<HttpRequestData>(Mock.Of<FunctionContext>());
        
        Stream bodyStream;
        if (body == null)
        {
            bodyStream = new MemoryStream();
        }
        else
        {
            var bytes = Encoding.UTF8.GetBytes(body);
            bodyStream = new MemoryStream(bytes);
        }
        
        mockRequest.Setup(r => r.Body).Returns(bodyStream);
        
        return mockRequest;
    }
}