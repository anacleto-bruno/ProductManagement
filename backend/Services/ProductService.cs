using Microsoft.Extensions.Logging;
using ProductManagement.dtos;
using ProductManagement.entities;
using ProductManagement.helpers;
using ProductManagement.infrastructure.repositories;
using ProductManagement.models;
using Bogus;

namespace ProductManagement.services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Result<ProductResponseDto>> GetByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
                return Result<ProductResponseDto>.Failure("Invalid product ID");

            var product = await _productRepository.GetProductDtoByIdAsync(id);
            return product != null
                ? Result<ProductResponseDto>.Success(product)
                : Result<ProductResponseDto>.Failure("Product not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product with ID {ProductId}", id);
            return Result<ProductResponseDto>.Failure("An error occurred while retrieving the product");
        }
    }

    public async Task<Result<PagedResultDto<ProductSummaryDto>>> GetPagedAsync(PaginationRequestDto request)
    {
        try
        {
            var result = await _productRepository.GetProductDtosAsync(request);
            return Result<PagedResultDto<ProductSummaryDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paginated products");
            return Result<PagedResultDto<ProductSummaryDto>>.Failure("An error occurred while retrieving products");
        }
    }

    public async Task<Result<ProductResponseDto>> CreateAsync(CreateProductRequestDto request)
    {
        try
        {
            // Check if SKU already exists
            if (await _productRepository.ExistsBySkuAsync(request.Sku))
            {
                return Result<ProductResponseDto>.Failure("A product with this SKU already exists");
            }

            var product = request.ToEntity();

            // Add color relationships
            if (request.ColorIds.Any())
            {
                var availableColors = await _productRepository.GetColorsAsync();
                var validColorIds = availableColors.Select(c => c.Id).ToHashSet();
                
                foreach (var colorId in request.ColorIds.Where(id => validColorIds.Contains(id)))
                {
                    product.ProductColors.Add(new ProductColor
                    {
                        Product = product,
                        ColorId = colorId
                    });
                }
            }

            // Add size relationships
            if (request.SizeIds.Any())
            {
                var availableSizes = await _productRepository.GetSizesAsync();
                var validSizeIds = availableSizes.Select(s => s.Id).ToHashSet();
                
                foreach (var sizeId in request.SizeIds.Where(id => validSizeIds.Contains(id)))
                {
                    product.ProductSizes.Add(new ProductSize
                    {
                        Product = product,
                        SizeId = sizeId
                    });
                }
            }

            var createdProduct = await _productRepository.AddAsync(product);
            var responseDto = await _productRepository.GetProductDtoByIdAsync(createdProduct.Id);
            
            return Result<ProductResponseDto>.Success(responseDto!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return Result<ProductResponseDto>.Failure("An error occurred while creating the product");
        }
    }

    public async Task<Result<ProductResponseDto>> UpdateAsync(int id, UpdateProductRequestDto request)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return Result<ProductResponseDto>.Failure("Product not found");

            // Check if SKU already exists (excluding current product)
            if (await _productRepository.ExistsBySkuAsync(request.Sku, id))
            {
                return Result<ProductResponseDto>.Failure("A product with this SKU already exists");
            }

            product.UpdateFromDto(request);

            // Update color relationships
            product.ProductColors.Clear();
            if (request.ColorIds.Any())
            {
                var availableColors = await _productRepository.GetColorsAsync();
                var validColorIds = availableColors.Select(c => c.Id).ToHashSet();
                
                foreach (var colorId in request.ColorIds.Where(id => validColorIds.Contains(id)))
                {
                    product.ProductColors.Add(new ProductColor
                    {
                        Product = product,
                        ColorId = colorId
                    });
                }
            }

            // Update size relationships
            product.ProductSizes.Clear();
            if (request.SizeIds.Any())
            {
                var availableSizes = await _productRepository.GetSizesAsync();
                var validSizeIds = availableSizes.Select(s => s.Id).ToHashSet();
                
                foreach (var sizeId in request.SizeIds.Where(id => validSizeIds.Contains(id)))
                {
                    product.ProductSizes.Add(new ProductSize
                    {
                        Product = product,
                        SizeId = sizeId
                    });
                }
            }

            await _productRepository.UpdateAsync(product);
            var responseDto = await _productRepository.GetProductDtoByIdAsync(id);
            
            return Result<ProductResponseDto>.Success(responseDto!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product with ID {ProductId}", id);
            return Result<ProductResponseDto>.Failure("An error occurred while updating the product");
        }
    }

    public async Task<Result> DeleteAsync(int id)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return Result.Failure("Product not found");

            await _productRepository.DeleteAsync(product);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product with ID {ProductId}", id);
            return Result.Failure("An error occurred while deleting the product");
        }
    }


}