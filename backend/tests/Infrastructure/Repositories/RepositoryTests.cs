using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductManagement.entities;
using ProductManagement.infrastructure;
using ProductManagement.infrastructure.repositories;
using Xunit;

namespace ProductManagement.Tests.Infrastructure.Repositories;

public class RepositoryTests
{
    private class TestEntity { public int Id { get; set; } public string Name { get; set; } = string.Empty; }

    private class TestDbContext : ApplicationDbContext
    {
        public TestDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<TestEntity> TestEntities => Set<TestEntity>();
    }

    private static TestDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"repo-tests-{System.Guid.NewGuid()}")
            .Options;
        return new TestDbContext(options);
    }

    [Fact]
    public async Task AddAsync_PersistsEntityAndReturnsIt()
    {
        using var ctx = CreateContext();
        var repo = new Repository<TestEntity>(ctx);
        var entity = new TestEntity { Name = "Alpha" };

        var result = await repo.AddAsync(entity);

        result.Should().BeSameAs(entity);
        (await ctx.Set<TestEntity>().CountAsync()).Should().Be(1);
        entity.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_WhenExists()
    {
        using var ctx = CreateContext();
        var repo = new Repository<TestEntity>(ctx);
        var saved = await repo.AddAsync(new TestEntity { Name = "Beta" });

        var found = await repo.GetByIdAsync(saved.Id);

        found.Should().NotBeNull();
        found!.Name.Should().Be("Beta");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEntities()
    {
        using var ctx = CreateContext();
        var repo = new Repository<TestEntity>(ctx);
        await repo.AddAsync(new TestEntity { Name = "A" });
        await repo.AddAsync(new TestEntity { Name = "B" });

        var all = (await repo.GetAllAsync()).ToList();

        all.Should().HaveCount(2).And.OnlyHaveUniqueItems(e => e.Id);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesTrackedEntity()
    {
        using var ctx = CreateContext();
        var repo = new Repository<TestEntity>(ctx);
        var entity = await repo.AddAsync(new TestEntity { Name = "Old" });

        entity.Name = "New";
        await repo.UpdateAsync(entity);

        var reloaded = await repo.GetByIdAsync(entity.Id);
        reloaded!.Name.Should().Be("New");
    }

    [Fact]
    public async Task DeleteAsync_ById_RemovesEntity()
    {
        using var ctx = CreateContext();
        var repo = new Repository<TestEntity>(ctx);
        var entity = await repo.AddAsync(new TestEntity { Name = "X" });

        await repo.DeleteAsync(entity.Id);

        (await repo.GetByIdAsync(entity.Id)).Should().BeNull();
        (await repo.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task DeleteAsync_ByEntity_RemovesEntity()
    {
        using var ctx = CreateContext();
        var repo = new Repository<TestEntity>(ctx);
        var entity = await repo.AddAsync(new TestEntity { Name = "Y" });

        await repo.DeleteAsync(entity);

        (await repo.GetByIdAsync(entity.Id)).Should().BeNull();
        (await repo.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task CountAsync_ReturnsTotalAndPredicateCounts()
    {
        using var ctx = CreateContext();
        var repo = new Repository<TestEntity>(ctx);
        await repo.AddAsync(new TestEntity { Name = "One" });
        await repo.AddAsync(new TestEntity { Name = "Two" });

        (await repo.CountAsync()).Should().Be(2);
        (await repo.CountAsync(e => e.Name.StartsWith("T"))).Should().Be(1);
    }

    [Fact]
    public async Task GetPagedAsync_NoPredicate_ReturnsRequestedPage()
    {
        using var ctx = CreateContext();
        var repo = new Repository<TestEntity>(ctx);
        for (int i = 0; i < 10; i++)
            await repo.AddAsync(new TestEntity { Name = $"N{i}" });

        var page = (await repo.GetPagedAsync(2, 3)).ToList();

        page.Should().HaveCount(3);
        page.Select(e => e.Name).Should().BeEquivalentTo(new[] { "N3", "N4", "N5" });
    }

    [Fact]
    public async Task GetPagedAsync_WithPredicate_FiltersThenPages()
    {
        using var ctx = CreateContext();
        var repo = new Repository<TestEntity>(ctx);
        for (int i = 0; i < 10; i++)
            await repo.AddAsync(new TestEntity { Name = i % 2 == 0 ? $"Even{i}" : $"Odd{i}" });

        var page = (await repo.GetPagedAsync(1, 2, e => e.Name.StartsWith("Even"))).ToList();

        page.Should().HaveCount(2);
        page.All(e => e.Name.StartsWith("Even")).Should().BeTrue();
    }
}
