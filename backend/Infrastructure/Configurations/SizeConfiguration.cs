using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManagement.entities;

namespace ProductManagement.infrastructure.configurations;

public class SizeConfiguration : IEntityTypeConfiguration<Size>
{
    public void Configure(EntityTypeBuilder<Size> builder)
    {
        builder.ToTable("sizes");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(10);
            
        builder.Property(x => x.SortOrder)
            .IsRequired();
            
        builder.HasIndex(x => x.Name)
            .HasDatabaseName("idx_sizes_name");
            
        builder.HasIndex(x => x.SortOrder)
            .HasDatabaseName("idx_sizes_sortorder");

        // Seed data directly in EF configuration
        builder.HasData(
            new Size { Id = 1, Name = "XS", SortOrder = 1, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Size { Id = 2, Name = "S", SortOrder = 2, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Size { Id = 3, Name = "M", SortOrder = 3, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Size { Id = 4, Name = "L", SortOrder = 4, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Size { Id = 5, Name = "XL", SortOrder = 5, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Size { Id = 6, Name = "XXL", SortOrder = 6, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Size { Id = 7, Name = "XXXL", SortOrder = 7, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}