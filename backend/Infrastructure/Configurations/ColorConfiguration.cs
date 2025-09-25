using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManagement.entities;

namespace ProductManagement.infrastructure.configurations;

public class ColorConfiguration : IEntityTypeConfiguration<Color>
{
    public void Configure(EntityTypeBuilder<Color> builder)
    {
        builder.ToTable("colors");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(x => x.HexCode)
            .IsRequired()
            .HasMaxLength(7);
            
        builder.HasIndex(x => x.Name)
            .HasDatabaseName("idx_colors_name");

        // Seed data directly in EF configuration
        builder.HasData(
            new Color { Id = 1, Name = "Red", HexCode = "#FF0000", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Color { Id = 2, Name = "Blue", HexCode = "#0000FF", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Color { Id = 3, Name = "Green", HexCode = "#008000", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Color { Id = 4, Name = "Yellow", HexCode = "#FFFF00", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Color { Id = 5, Name = "Orange", HexCode = "#FFA500", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Color { Id = 6, Name = "Purple", HexCode = "#800080", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Color { Id = 7, Name = "Pink", HexCode = "#FFC0CB", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Color { Id = 8, Name = "Brown", HexCode = "#A52A2A", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Color { Id = 9, Name = "Black", HexCode = "#000000", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Color { Id = 10, Name = "White", HexCode = "#FFFFFF", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Color { Id = 11, Name = "Gray", HexCode = "#808080", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Color { Id = 12, Name = "Navy", HexCode = "#000080", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Color { Id = 13, Name = "Maroon", HexCode = "#800000", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Color { Id = 14, Name = "Teal", HexCode = "#008080", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Color { Id = 15, Name = "Silver", HexCode = "#C0C0C0", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}