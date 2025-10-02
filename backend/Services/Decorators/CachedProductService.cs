using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
    private readonly CacheOptions _cacheOptions;

    public CachedProductService(IProductService inner, ICacheService cache, ILogger<CachedProductService> logger, IOptions<CacheOptions> cacheOptions)
    {
        _inner = inner;
        _cache = cache;
        _logger = logger;
        _cacheOptions = cacheOptions.Value;
    }

    public async Task<Result<ProductResponseDto>> GetByIdAsync(int id)
    {
        if (!_cacheOptions.Enabled)
        {
            return await _inner.GetByIdAsync(id);
        }

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
            await _cache.SetAsync(cacheKey, result.Data, _cacheOptions.DefaultTtl);
        }
        return result;
    }

    public async Task<Result<PagedResultDto<ProductSummaryDto>>> GetPagedAsync(PaginationRequestDto request)
    {
        if (!_cacheOptions.Enabled)
        {
            return await _inner.GetPagedAsync(request);
        }

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
            await _cache.SetAsync(cacheKey, result.Data, _cacheOptions.DefaultTtl);
            await _cache.AddToSetAsync(ProductCacheKeys.PagedIndexSet, cacheKey);
        }
        return result;
    }

    public async Task<Result<ProductResponseDto>> CreateAsync(CreateProductRequestDto request)
    {
        var result = await _inner.CreateAsync(request);
        if (result.IsSuccess && result.Data is not null && _cacheOptions.Enabled)
        {
            await InvalidatePagedAsync();
            // ProductById will be cached lazily on first read
        }
        return result;
    }

    public async Task<Result<ProductResponseDto>> UpdateAsync(int id, UpdateProductRequestDto request)
    {
        var result = await _inner.UpdateAsync(id, request);
        if (result.IsSuccess && result.Data is not null && _cacheOptions.Enabled)
        {
            await _cache.RemoveAsync(ProductCacheKeys.ById(id));
            await InvalidatePagedAsync();
        }
        return result;
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var result = await _inner.DeleteAsync(id);
        if (result.IsSuccess && _cacheOptions.Enabled)
        {
            await _cache.RemoveAsync(ProductCacheKeys.ById(id));
            await InvalidatePagedAsync();
        }
        return result;
    }

    public async Task<Result<List<ProductResponseDto>>> SeedAsync(int count)
    {
        var result = await _inner.SeedAsync(count);
        if (result.IsSuccess && _cacheOptions.Enabled)
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