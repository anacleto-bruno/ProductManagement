using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using ProductManagement.services.caching;

namespace ProductManagement.Tests.Services.Caching;

public class NoOpCacheServiceTests
{
    private readonly Mock<ILogger<NoOpCacheService>> _mockLogger;
    private readonly NoOpCacheService _cacheService;

    public NoOpCacheServiceTests()
    {
        _mockLogger = new Mock<ILogger<NoOpCacheService>>();
        _cacheService = new NoOpCacheService(_mockLogger.Object);
    }

    [Fact]
    public async Task GetAsync_AlwaysReturnsDefault()
    {
        // Act
        var stringResult = await _cacheService.GetAsync<string>("test-key");
        var intResult = await _cacheService.GetAsync<int>("test-key");
        var objectResult = await _cacheService.GetAsync<TestObject>("test-key");

        // Assert
        stringResult.Should().BeNull();
        intResult.Should().Be(default(int));
        objectResult.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_DoesNotThrow()
    {
        // Arrange
        var testObject = new TestObject { Name = "Test", Value = 42 };

        // Act & Assert (should not throw)
        await _cacheService.SetAsync("test-key", testObject);
        await _cacheService.SetAsync("test-key", testObject, TimeSpan.FromMinutes(5));
    }

    [Fact]
    public async Task RemoveAsync_SingleKey_DoesNotThrow()
    {
        // Act & Assert (should not throw)
        await _cacheService.RemoveAsync("test-key");
    }

    [Fact]
    public async Task RemoveAsync_MultipleKeys_DoesNotThrow()
    {
        // Arrange
        var keys = new[] { "key1", "key2", "key3" };

        // Act & Assert (should not throw)
        await _cacheService.RemoveAsync(keys);
    }

    [Fact]
    public async Task AddToSetAsync_DoesNotThrow()
    {
        // Act & Assert (should not throw)
        await _cacheService.AddToSetAsync("set-key", "member1");
    }

    [Fact]
    public async Task GetSetMembersAsync_ReturnsEmptyArray()
    {
        // Act
        var result = await _cacheService.GetSetMembersAsync("set-key");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    private class TestObject
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}