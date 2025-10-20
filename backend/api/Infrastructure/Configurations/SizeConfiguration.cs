using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManagement.entities;

namespace ProductManagement.infrastructure.configurations;

public class SizeConfiguration : IEntityTypeConfiguration<Size>
{
    public void Configure(EntityTypeBuilder<Size> builder)
    {
        builder.ToTable("sizes");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(s => s.Code)
            .HasMaxLength(10);
            
        builder.Property(s => s.SortOrder)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .IsRequired();
            
        // Performance indexes
        builder.HasIndex(s => s.Name)
            .HasDatabaseName("idx_sizes_name");
            
        builder.HasIndex(s => s.SortOrder)
            .HasDatabaseName("idx_sizes_sortorder");

        // Configure relationships
        builder.HasMany(s => s.ProductSizes)
            .WithOne(ps => ps.Size)
            .HasForeignKey(ps => ps.SizeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed data
        builder.HasData(
            new Size { Id = 1, Name = "XS", Code = "XS", SortOrder = 1, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Size { Id = 2, Name = "S", Code = "S", SortOrder = 2, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Size { Id = 3, Name = "M", Code = "M", SortOrder = 3, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Size { Id = 4, Name = "L", Code = "L", SortOrder = 4, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Size { Id = 5, Name = "XL", Code = "XL", SortOrder = 5, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Size { Id = 6, Name = "XXL", Code = "XXL", SortOrder = 6, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Size { Id = 7, Name = "XXXL", Code = "XXXL", SortOrder = 7, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}