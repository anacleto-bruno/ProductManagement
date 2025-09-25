using ProductManagement.dtos;
using ProductManagement.entities;

namespace ProductManagement.infrastructure.repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<ProductResponseDto?> GetProductDtoByIdAsync(int id);
    Task<PagedResultDto<ProductSummaryDto>> GetProductDtosAsync(PaginationRequestDto request);
    Task<bool> ExistsBySkuAsync(string sku, int? excludeId = null);
    Task<List<Color>> GetColorsAsync();
    Task<List<Size>> GetSizesAsync();
}