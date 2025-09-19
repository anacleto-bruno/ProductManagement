using ProductManagement.Dtos;
using ProductManagement.Entities;

namespace ProductManagement.Infrastructure.Repositories.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<ProductResponseDto?> GetProductDtoByIdAsync(int id);

    Task<IEnumerable<ProductResponseDto>> GetProductDtosAsync(
        string? searchTerm = null,
        string? category = null,
        string? brand = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? sortBy = null,
        bool descending = false,
        int page = 1,
        int pageSize = 20);

    Task<int> GetProductCountAsync(
        string? searchTerm = null,
        string? category = null,
        string? brand = null,
        decimal? minPrice = null,
        decimal? maxPrice = null);

    Task<bool> ExistsBySkuAsync(string sku, int? excludeId = null);
}