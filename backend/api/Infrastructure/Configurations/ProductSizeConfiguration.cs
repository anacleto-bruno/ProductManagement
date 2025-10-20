using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManagement.entities;

namespace ProductManagement.infrastructure.configurations;

public class ProductSizeConfiguration : IEntityTypeConfiguration<ProductSize>
{
    public void Configure(EntityTypeBuilder<ProductSize> builder)
    {
        builder.ToTable("productsizes");

        // Composite primary key
        builder.HasKey(ps => new { ps.ProductId, ps.SizeId });

        builder.Property(ps => ps.StockQuantity)
            .HasDefaultValue(0);

        builder.Property(ps => ps.CreatedAt)
            .IsRequired();

        // Configure relationships
        builder.HasOne(ps => ps.Product)
            .WithMany(p => p.ProductSizes)
            .HasForeignKey(ps => ps.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ps => ps.Size)
            .WithMany(s => s.ProductSizes)
            .HasForeignKey(ps => ps.SizeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        builder.HasIndex(ps => ps.ProductId)
            .HasDatabaseName("idx_productsizes_productid");

        builder.HasIndex(ps => ps.SizeId)
            .HasDatabaseName("idx_productsizes_sizeid");
    }
}
