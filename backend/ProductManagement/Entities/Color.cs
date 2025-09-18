namespace ProductManagement.Entities;

public class Color : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string HexCode { get; set; } = string.Empty;

    public ICollection<ProductColor> ProductColors { get; set; } = new List<ProductColor>();
}