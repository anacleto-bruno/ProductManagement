using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManagement.Entities;

namespace ProductManagement.Infrastructure.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.Brand)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Sku)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Price)
            .HasPrecision(18, 2);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        // Performance indexes
        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => p.Description);
        builder.HasIndex(p => p.Brand);
        builder.HasIndex(p => p.Category);
        builder.HasIndex(p => p.Sku).IsUnique();
        builder.HasIndex(p => p.Price);

        // Composite indexes for common query patterns
        builder.HasIndex(p => new { p.Category, p.Brand });
        builder.HasIndex(p => new { p.Category, p.Price });
        builder.HasIndex(p => new { p.Brand, p.Price });
        builder.HasIndex(p => new { p.CreatedAt, p.Category });

        // Text search indexes (PostgreSQL specific)
        builder.HasIndex(p => new { p.Name, p.Description, p.Brand });

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