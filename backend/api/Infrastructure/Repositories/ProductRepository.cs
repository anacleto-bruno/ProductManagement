using Microsoft.EntityFrameworkCore;
using ProductManagement.dtos;
using ProductManagement.entities;
using ProductManagement.helpers;

namespace ProductManagement.infrastructure.repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context) { }

    public async Task<ProductResponseDto?> GetProductDtoByIdAsync(int id)
    {
        return await _context.Set<Product>()
            .AsNoTracking()
            .Where(p => p.Id == id)
            .IncludeRelated()
            .Select(MappingExtensions.ToResponseDtoExpression)
            .FirstOrDefaultAsync();
    }

    public async Task<PagedResultDto<ProductSummaryDto>> GetProductDtosAsync(PaginationRequestDto request)
    {
        var query = _context.Set<Product>().AsNoTracking();

        // Apply filters using extension methods
        query = query.WhereSearch(request.SearchTerm);
        query = query.WhereCategory(request.Category);
        query = query.WherePriceRange(request.MinPrice, request.MaxPrice);

        // Get total count for pagination
        var totalCount = await query.CountAsync();

        // Apply sorting and pagination
        query = query.OrderByField(request.SortBy, request.Descending);

        var products = await query
            .IncludeRelated()
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(MappingExtensions.ToSummaryDtoExpression)
            .ToListAsync();

        return new PagedResultDto<ProductSummaryDto>
        {
            Data = products,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    public async Task<bool> ExistsBySkuAsync(string sku, int? excludeId = null)
    {
        var query = _context.Set<Product>().AsNoTracking().Where(p => p.Sku == sku);
        
        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<List<Color>> GetColorsAsync()
    {
        return await _context.Set<Color>()
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<List<Size>> GetSizesAsync()
    {
        return await _context.Set<Size>()
            .AsNoTracking()
            .OrderBy(s => s.SortOrder ?? int.MaxValue)
            .ThenBy(s => s.Name)
            .ToListAsync();
    }

    public override async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Set<Product>()
            .IncludeRelated()
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}