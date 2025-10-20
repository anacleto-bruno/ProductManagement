using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManagement.entities;

namespace ProductManagement.infrastructure.configurations;

public class ProductColorConfiguration : IEntityTypeConfiguration<ProductColor>
{
    public void Configure(EntityTypeBuilder<ProductColor> builder)
    {
        builder.ToTable("productcolors");

        // Composite primary key
        builder.HasKey(pc => new { pc.ProductId, pc.ColorId });

        builder.Property(pc => pc.CreatedAt)
            .IsRequired();

        // Configure relationships
        builder.HasOne(pc => pc.Product)
            .WithMany(p => p.ProductColors)
            .HasForeignKey(pc => pc.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pc => pc.Color)
            .WithMany(c => c.ProductColors)
            .HasForeignKey(pc => pc.ColorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        builder.HasIndex(pc => pc.ProductId)
            .HasDatabaseName("idx_productcolors_productid");

        builder.HasIndex(pc => pc.ColorId)
            .HasDatabaseName("idx_productcolors_colorid");
    }
}
