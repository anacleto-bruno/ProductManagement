namespace ProductManagement.Entities;

public class Size : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
}