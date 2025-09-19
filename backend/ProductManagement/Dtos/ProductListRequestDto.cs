namespace ProductManagement.Dtos;

public class ProductListRequestDto
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool Descending { get; set; } = false;
    public string? Search { get; set; }
    public string? Category { get; set; }
    public string? Brand { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}