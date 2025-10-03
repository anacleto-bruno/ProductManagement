using System.ComponentModel.DataAnnotations;

namespace ProductManagement.entities;

public class Product
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Model { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Brand { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Sku { get; set; } = string.Empty;
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    
    [MaxLength(100)]
    public string? Category { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<ProductColor> ProductColors { get; set; } = new List<ProductColor>();
    public virtual ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
}