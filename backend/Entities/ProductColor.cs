namespace ProductManagement.entities;

public class ProductColor
{
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    
    public int ColorId { get; set; }
    public virtual Color Color { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}