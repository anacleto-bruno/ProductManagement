using Microsoft.EntityFrameworkCore;
using ProductManagement.entities;
using ProductManagement.infrastructure.configurations;

namespace ProductManagement.infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Color> Colors { get; set; }
    public DbSet<Size> Sizes { get; set; }
    public DbSet<ProductColor> ProductColors { get; set; }
    public DbSet<ProductSize> ProductSizes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductConfiguration).Assembly);

        // Configure naming convention for PostgreSQL
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.GetTableName()?.ToLowerInvariant());
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName().ToLowerInvariant());
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Automatic timestamp handling
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Product && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                ((Product)entry.Entity).CreatedAt = DateTime.UtcNow;
            }
            ((Product)entry.Entity).UpdatedAt = DateTime.UtcNow;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}