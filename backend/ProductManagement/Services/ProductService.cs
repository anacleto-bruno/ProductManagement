using ProductManagement.Dtos;
using ProductManagement.Entities;
using ProductManagement.Extensions;
using ProductManagement.Helpers;
using ProductManagement.Infrastructure.Repositories.Interfaces;
using ProductManagement.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductManagement.Models.Configuration;

namespace ProductManagement.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly ILogger<ProductService> _logger;
    private readonly CacheSettingsConfig _cacheSettings;

    public ProductService(
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        ILogger<ProductService> logger,
        IOptions<CacheSettingsConfig> cacheSettings)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _logger = logger;
        _cacheSettings = cacheSettings.Value;
    }

    public async Task<Result<ProductResponseDto>> CreateProductAsync(CreateProductRequestDto request)
    {
        try
        {
            var product = request.ToEntity();

            var createdProduct = await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            var response = createdProduct.ToResponseDto();

            // Invalidate search caches since a new product was created
            await _cacheService.InvalidateProductSearchCacheAsync();
            _logger.LogInformation("Created product {ProductId} and invalidated search cache", createdProduct.Id);

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
            // Try to get from cache first
            var cacheKey = _cacheService.GenerateProductDetailKey(id);
            var cachedProduct = await _cacheService.GetAsync<ProductResponseDto>(cacheKey);

            if (cachedProduct != null)
            {
                _logger.LogDebug("Product {ProductId} retrieved from cache", id);
                return Result<ProductResponseDto>.Success(cachedProduct);
            }

            // Cache miss - get from database
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product == null)
            {
                return Result<ProductResponseDto>.Failure("Product not found");
            }

            var response = product.ToResponseDto();

            // Cache the result
            var ttl = TimeSpan.FromMinutes(_cacheSettings.ProductDetailTtlMinutes);
            await _cacheService.SetAsync(cacheKey, response, ttl);
            _logger.LogDebug("Product {ProductId} cached with TTL {TTL}", id, ttl);

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

            // Invalidate caches for this product
            await _cacheService.InvalidateProductCacheAsync(id);
            _logger.LogInformation("Updated product {ProductId} and invalidated related caches", id);

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

            // Invalidate caches for this product
            await _cacheService.InvalidateProductCacheAsync(id);
            _logger.LogInformation("Deleted product {ProductId} and invalidated related caches", id);

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
            // Generate cache key based on search parameters
            var cacheKey = _cacheService.GenerateProductSearchKey(request);

            // Try to get from cache first
            var cachedResult = await _cacheService.GetAsync<PagedResultDto<ProductResponseDto>>(cacheKey);

            if (cachedResult != null)
            {
                _logger.LogDebug("Product search results retrieved from cache for key: {CacheKey}", cacheKey);
                return Result<PagedResultDto<ProductResponseDto>>.Success(cachedResult);
            }

            // Cache miss - get from database
            _logger.LogDebug("Product search cache miss for key: {CacheKey}, querying database", cacheKey);
            var result = await _unitOfWork.Products.GetPagedProductsAsync(request);

            // Cache the result with configured TTL
            var ttl = TimeSpan.FromMinutes(_cacheSettings.ProductSearchTtlMinutes);
            await _cacheService.SetAsync(cacheKey, result, ttl);

            _logger.LogInformation("Product search results cached for key: {CacheKey}, TTL: {TTL}, Results: {Count}/{Total}",
                cacheKey, ttl, result.Data.Count(), result.Pagination.TotalCount);

            return Result<PagedResultDto<ProductResponseDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve products with search parameters: {@Request}", request);
            return Result<PagedResultDto<ProductResponseDto>>.Failure($"Failed to retrieve products: {ex.Message}");
        }
    }
}