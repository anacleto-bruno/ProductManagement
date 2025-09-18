using Microsoft.EntityFrameworkCore;
using ProductManagement.Entities;

namespace ProductManagement.Infrastructure.Data;

public class ProductManagementContext : DbContext
{
    public ProductManagementContext(DbContextOptions<ProductManagementContext> options) : base(options)
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

        // Configure naming convention for PostgreSQL
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.GetTableName()?.ToLowerInvariant());

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName().ToLowerInvariant());
            }
        }

        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(255);
            entity.Property(p => p.Description).HasMaxLength(1000);
            entity.Property(p => p.Brand).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Sku).IsRequired().HasMaxLength(50);
            entity.Property(p => p.Category).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Price).HasPrecision(18, 2);
            entity.Property(p => p.CreatedAt).IsRequired();
            entity.Property(p => p.UpdatedAt).IsRequired();

            // Indexes for performance
            entity.HasIndex(p => p.Name);
            entity.HasIndex(p => p.Description);
            entity.HasIndex(p => p.Brand);
            entity.HasIndex(p => p.Category);
            entity.HasIndex(p => p.Sku).IsUnique();
            entity.HasIndex(p => p.Price);

            // Composite indexes for common query patterns
            entity.HasIndex(p => new { p.Category, p.Brand });
            entity.HasIndex(p => new { p.Category, p.Price });
            entity.HasIndex(p => new { p.Brand, p.Price });
            entity.HasIndex(p => new { p.CreatedAt, p.Category });

            // Text search indexes (PostgreSQL specific)
            entity.HasIndex(p => new { p.Name, p.Description, p.Brand });
        });

        // Configure Color entity
        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(50);
            entity.Property(c => c.HexCode).IsRequired().HasMaxLength(7);
            entity.Property(c => c.CreatedAt).IsRequired();
            entity.Property(c => c.UpdatedAt).IsRequired();

            entity.HasIndex(c => c.Name).IsUnique();
            entity.HasIndex(c => c.HexCode);
        });

        // Configure Size entity
        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Name).IsRequired().HasMaxLength(50);
            entity.Property(s => s.Value).IsRequired().HasMaxLength(20);
            entity.Property(s => s.CreatedAt).IsRequired();
            entity.Property(s => s.UpdatedAt).IsRequired();

            entity.HasIndex(s => s.Name);
        });

        // Configure ProductColor junction table
        modelBuilder.Entity<ProductColor>(entity =>
        {
            entity.HasKey(pc => new { pc.ProductId, pc.ColorId });

            entity.HasOne(pc => pc.Product)
                .WithMany(p => p.ProductColors)
                .HasForeignKey(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(pc => pc.Color)
                .WithMany(c => c.ProductColors)
                .HasForeignKey(pc => pc.ColorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure ProductSize junction table
        modelBuilder.Entity<ProductSize>(entity =>
        {
            entity.HasKey(ps => new { ps.ProductId, ps.SizeId });

            entity.HasOne(ps => ps.Product)
                .WithMany(p => p.ProductSizes)
                .HasForeignKey(ps => ps.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ps => ps.Size)
                .WithMany(s => s.ProductSizes)
                .HasForeignKey(ps => ps.SizeId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
    }
}