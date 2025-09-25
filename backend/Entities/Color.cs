using System.ComponentModel.DataAnnotations;

namespace ProductManagement.entities;

public class Color
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(7)] // For hex color codes like #FF0000
    public string? HexCode { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<ProductColor> ProductColors { get; set; } = new List<ProductColor>();
}