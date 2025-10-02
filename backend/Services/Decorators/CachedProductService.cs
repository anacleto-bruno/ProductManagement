using Microsoft.Extensions.Logging;
using ProductManagement.dtos;
using ProductManagement.models;
using ProductManagement.services.caching;

namespace ProductManagement.services.decorators;

/// <summary>
/// Decorator adding Redis-based caching to product read operations and invalidation on mutations.
/// </summary>
public class CachedProductService : IProductService
{
    private readonly IProductService _inner;
    private readonly ICacheService _cache;
    private readonly ILogger<CachedProductService> _logger;

    private static readonly TimeSpan ProductByIdTtl = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan ProductPagedTtl = TimeSpan.FromSeconds(45);

    public CachedProductService(IProductService inner, ICacheService cache, ILogger<CachedProductService> logger)
    {
        _inner = inner;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Result<ProductResponseDto>> GetByIdAsync(int id)
    {
        var cacheKey = ProductCacheKeys.ById(id);
        var cached = await _cache.GetAsync<ProductResponseDto>(cacheKey);
        if (cached is not null)
        {
            _logger.LogDebug("Cache HIT product by id {Id}", id);
            return Result<ProductResponseDto>.Success(cached);
        }

        var result = await _inner.GetByIdAsync(id);
        if (result.IsSuccess && result.Data is not null)
        {
            await _cache.SetAsync(cacheKey, result.Data, ProductByIdTtl);
        }
        return result;
    }

    public async Task<Result<PagedResultDto<ProductSummaryDto>>> GetPagedAsync(PaginationRequestDto request)
    {
        var cacheKey = ProductCacheKeys.Paged(request);
        var cached = await _cache.GetAsync<PagedResultDto<ProductSummaryDto>>(cacheKey);
        if (cached is not null)
        {
            _logger.LogDebug("Cache HIT products paged (page {Page}, size {Size})", request.Page, request.PageSize);
            return Result<PagedResultDto<ProductSummaryDto>>.Success(cached);
        }

        var result = await _inner.GetPagedAsync(request);
        if (result.IsSuccess && result.Data is not null)
        {
            await _cache.SetAsync(cacheKey, result.Data, ProductPagedTtl);
            await _cache.AddToSetAsync(ProductCacheKeys.PagedIndexSet, cacheKey);
        }
        return result;
    }

    public async Task<Result<ProductResponseDto>> CreateAsync(CreateProductRequestDto request)
    {
        var result = await _inner.CreateAsync(request);
        if (result.IsSuccess && result.Data is not null)
        {
            await InvalidatePagedAsync();
            // ProductById will be cached lazily on first read
        }
        return result;
    }

    public async Task<Result<ProductResponseDto>> UpdateAsync(int id, UpdateProductRequestDto request)
    {
        var result = await _inner.UpdateAsync(id, request);
        if (result.IsSuccess && result.Data is not null)
        {
            await _cache.RemoveAsync(ProductCacheKeys.ById(id));
            await InvalidatePagedAsync();
        }
        return result;
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var result = await _inner.DeleteAsync(id);
        if (result.IsSuccess)
        {
            await _cache.RemoveAsync(ProductCacheKeys.ById(id));
            await InvalidatePagedAsync();
        }
        return result;
    }

    public async Task<Result<List<ProductResponseDto>>> SeedAsync(int count)
    {
        var result = await _inner.SeedAsync(count);
        if (result.IsSuccess)
        {
            await InvalidatePagedAsync();
        }
        return result;
    }

    private async Task InvalidatePagedAsync()
    {
        try
        {
            var keys = await _cache.GetSetMembersAsync(ProductCacheKeys.PagedIndexSet);
            if (keys.Length > 0)
            {
                await _cache.RemoveAsync(keys);
                _logger.LogDebug("Invalidated {Count} paged product cache entries", keys.Length);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to invalidate paged product cache");
        }
    }
}