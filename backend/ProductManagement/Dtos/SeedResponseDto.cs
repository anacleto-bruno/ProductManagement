namespace ProductManagement.Dtos;

public class SeedResponseDto
{
    public int ProductsCreated { get; set; }
    public int ColorsCreated { get; set; }
    public int SizesCreated { get; set; }
    public int ProductColorRelationsCreated { get; set; }
    public int ProductSizeRelationsCreated { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public string Message { get; set; } = string.Empty;
}