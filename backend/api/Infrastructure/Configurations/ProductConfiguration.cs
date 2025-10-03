using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManagement.entities;

namespace ProductManagement.infrastructure.configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(255);

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

        builder.Property(p => p.Description);

        // Performance indexes
        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => p.Sku).IsUnique();
        builder.HasIndex(p => p.Brand);
        builder.HasIndex(p => p.Category);
        builder.HasIndex(p => p.Price);

        // Composite indexes for common query patterns
        builder.HasIndex(p => new { p.Category, p.Brand });
        builder.HasIndex(p => new { p.Brand, p.Model });

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