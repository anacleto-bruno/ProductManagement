using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManagement.entities;

namespace ProductManagement.infrastructure.configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Brand)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Sku)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.Category)
            .HasMaxLength(100);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        // Performance indexes
        builder.HasIndex(p => p.Name)
            .HasDatabaseName("idx_products_name");

        builder.HasIndex(p => p.Sku)
            .IsUnique()
            .HasDatabaseName("idx_products_sku");

        builder.HasIndex(p => p.Brand)
            .HasDatabaseName("idx_products_brand");

        builder.HasIndex(p => p.Category)
            .HasDatabaseName("idx_products_category");

        builder.HasIndex(p => p.Price)
            .HasDatabaseName("idx_products_price");

        builder.HasIndex(p => p.CreatedAt)
            .HasDatabaseName("idx_products_createdat");

        // Composite indexes for common query patterns
        builder.HasIndex(p => new { p.Category, p.Brand })
            .HasDatabaseName("idx_products_category_brand");

        builder.HasIndex(p => new { p.Brand, p.Model })
            .HasDatabaseName("idx_products_brand_model");

        // Configure relationships
        builder.HasMany(p => p.ProductColors)
            .WithOne(pc => pc.Product)
            .HasForeignKey(pc => pc.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.ProductSizes)
            .WithOne(ps => ps.Product)
            .HasForeignKey(ps => ps.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}