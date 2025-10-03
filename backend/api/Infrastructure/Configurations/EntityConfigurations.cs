using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManagement.entities;

namespace ProductManagement.infrastructure;

public class ColorConfiguration : IEntityTypeConfiguration<Color>
{
    public void Configure(EntityTypeBuilder<Color> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.HexCode)
            .HasMaxLength(7);

        // Performance indexes
        builder.HasIndex(c => c.Name).IsUnique();

        // Configure relationships
        builder.HasMany(c => c.ProductColors)
            .WithOne(pc => pc.Color)
            .HasForeignKey(pc => pc.ColorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class SizeConfiguration : IEntityTypeConfiguration<Size>
{
    public void Configure(EntityTypeBuilder<Size> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.Code)
            .HasMaxLength(10);

        // Performance indexes
        builder.HasIndex(s => s.Name).IsUnique();
        builder.HasIndex(s => s.SortOrder);

        // Configure relationships
        builder.HasMany(s => s.ProductSizes)
            .WithOne(ps => ps.Size)
            .HasForeignKey(ps => ps.SizeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ProductColorConfiguration : IEntityTypeConfiguration<ProductColor>
{
    public void Configure(EntityTypeBuilder<ProductColor> builder)
    {
        // Composite primary key
        builder.HasKey(pc => new { pc.ProductId, pc.ColorId });

        // Configure relationships
        builder.HasOne(pc => pc.Product)
            .WithMany(p => p.ProductColors)
            .HasForeignKey(pc => pc.ProductId);

        builder.HasOne(pc => pc.Color)
            .WithMany(c => c.ProductColors)
            .HasForeignKey(pc => pc.ColorId);
    }
}

public class ProductSizeConfiguration : IEntityTypeConfiguration<ProductSize>
{
    public void Configure(EntityTypeBuilder<ProductSize> builder)
    {
        // Composite primary key
        builder.HasKey(ps => new { ps.ProductId, ps.SizeId });

        builder.Property(ps => ps.StockQuantity)
            .HasDefaultValue(0);

        // Configure relationships
        builder.HasOne(ps => ps.Product)
            .WithMany(p => p.ProductSizes)
            .HasForeignKey(ps => ps.ProductId);

        builder.HasOne(ps => ps.Size)
            .WithMany(s => s.ProductSizes)
            .HasForeignKey(ps => ps.SizeId);
    }
}