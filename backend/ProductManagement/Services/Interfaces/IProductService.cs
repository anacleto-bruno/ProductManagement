using ProductManagement.Dtos;
using ProductManagement.Helpers;

namespace ProductManagement.Services.Interfaces;

public interface IProductService
{
    Task<Result<ProductResponseDto>> CreateProductAsync(CreateProductRequestDto request);
    Task<Result<ProductResponseDto>> GetProductByIdAsync(int id);
    Task<Result<ProductResponseDto>> UpdateProductAsync(int id, UpdateProductRequestDto request);
    Task<Result<bool>> DeleteProductAsync(int id);
}