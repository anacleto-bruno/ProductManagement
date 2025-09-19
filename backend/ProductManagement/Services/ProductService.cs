using ProductManagement.Dtos;
using ProductManagement.Entities;
using ProductManagement.Extensions;
using ProductManagement.Helpers;
using ProductManagement.Infrastructure.Repositories.Interfaces;
using ProductManagement.Services.Interfaces;

namespace ProductManagement.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductResponseDto>> CreateProductAsync(CreateProductRequestDto request)
    {
        try
        {
            var product = request.ToEntity();

            var createdProduct = await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            var response = createdProduct.ToResponseDto();
            return Result<ProductResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<ProductResponseDto>.Failure($"Failed to create product: {ex.Message}");
        }
    }

    public async Task<Result<ProductResponseDto>> GetProductByIdAsync(int id)
    {
        try
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product == null)
            {
                return Result<ProductResponseDto>.Failure("Product not found");
            }

            var response = product.ToResponseDto();
            return Result<ProductResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<ProductResponseDto>.Failure($"Failed to retrieve product: {ex.Message}");
        }
    }

    public async Task<Result<ProductResponseDto>> UpdateProductAsync(int id, UpdateProductRequestDto request)
    {
        try
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product == null)
            {
                return Result<ProductResponseDto>.Failure("Product not found");
            }

            product.UpdateFromDto(request);

            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();

            var response = product.ToResponseDto();
            return Result<ProductResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<ProductResponseDto>.Failure($"Failed to update product: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteProductAsync(int id)
    {
        try
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product == null)
            {
                return Result<bool>.Failure("Product not found");
            }

            await _unitOfWork.Products.DeleteAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to delete product: {ex.Message}");
        }
    }

    public async Task<Result<PagedResultDto<ProductResponseDto>>> GetProductsAsync(ProductListRequestDto request)
    {
        try
        {
            var result = await _unitOfWork.Products.GetPagedProductsAsync(request);
            return Result<PagedResultDto<ProductResponseDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PagedResultDto<ProductResponseDto>>.Failure($"Failed to retrieve products: {ex.Message}");
        }
    }
}