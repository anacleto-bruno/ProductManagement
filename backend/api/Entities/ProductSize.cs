namespace ProductManagement.entities;

public class ProductSize
{
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    
    public int SizeId { get; set; }
    public virtual Size Size { get; set; } = null!;
    
    public int? StockQuantity { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}