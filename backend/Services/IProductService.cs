using ProductManagement.dtos;
using ProductManagement.models;

namespace ProductManagement.services;

public interface IProductService
{
    Task<Result<ProductResponseDto>> GetByIdAsync(int id);
    Task<Result<PagedResultDto<ProductSummaryDto>>> GetPagedAsync(PaginationRequestDto request);
    Task<Result<ProductResponseDto>> CreateAsync(CreateProductRequestDto request);
    Task<Result<ProductResponseDto>> UpdateAsync(int id, UpdateProductRequestDto request);
    Task<Result> DeleteAsync(int id);

}