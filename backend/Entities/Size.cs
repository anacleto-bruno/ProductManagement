using System.ComponentModel.DataAnnotations;

namespace ProductManagement.entities;

public class Size
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(10)]
    public string? Code { get; set; } // S, M, L, XL, etc.
    
    public int? SortOrder { get; set; } // For display ordering
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
}