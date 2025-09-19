using Microsoft.EntityFrameworkCore;
using ProductManagement.Dtos;
using ProductManagement.Entities;
using ProductManagement.Infrastructure.Data;
using ProductManagement.Infrastructure.Extensions;
using ProductManagement.Infrastructure.Repositories.Interfaces;

namespace ProductManagement.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ProductManagementContext context) : base(context)
    {
    }

    public async Task<ProductResponseDto?> GetProductDtoByIdAsync(int id)
    {
        return await _context.Products
            .Where(p => p.Id == id)
            .Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Brand = p.Brand,
                Sku = p.Sku,
                Price = p.Price,
                Category = p.Category,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ProductResponseDto>> GetProductDtosAsync(
        string? searchTerm = null,
        string? category = null,
        string? brand = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? sortBy = null,
        bool descending = false,
        int page = 1,
        int pageSize = 20)
    {
        var query = _context.Products
            .WhereSearch(searchTerm!)
            .WhereCategory(category!)
            .WhereBrand(brand!)
            .WherePriceRange(minPrice, maxPrice)
            .OrderByField(sortBy!, descending);

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Brand = p.Brand,
                Sku = p.Sku,
                Price = p.Price,
                Category = p.Category,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<int> GetProductCountAsync(
        string? searchTerm = null,
        string? category = null,
        string? brand = null,
        decimal? minPrice = null,
        decimal? maxPrice = null)
    {
        return await _context.Products
            .WhereSearch(searchTerm!)
            .WhereCategory(category!)
            .WhereBrand(brand!)
            .WherePriceRange(minPrice, maxPrice)
            .CountAsync();
    }

    public async Task<bool> ExistsBySkuAsync(string sku, int? excludeId = null)
    {
        var query = _context.Products.Where(p => p.Sku == sku);

        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task<PagedResultDto<ProductResponseDto>> GetPagedProductsAsync(ProductListRequestDto request)
    {
        var query = _context.Products
            .AsNoTracking()
            .WhereSearch(request.Search!)
            .WhereCategory(request.Category!)
            .WhereBrand(request.Brand!)
            .WherePriceRange(request.MinPrice, request.MaxPrice);

        // Get total count for metadata
        var totalCount = await query.CountAsync();

        // Apply sorting and pagination
        var pagedQuery = query
            .OrderByField(request.SortBy!, request.Descending)
            .Skip((request.Page - 1) * request.PerPage)
            .Take(request.PerPage);

        // Get the data with projection
        var data = await pagedQuery
            .Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Brand = p.Brand,
                Sku = p.Sku,
                Price = p.Price,
                Category = p.Category,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync();

        // Calculate pagination metadata
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PerPage);

        var pagination = new PaginationMetadataDto
        {
            CurrentPage = request.Page,
            PerPage = request.PerPage,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasNextPage = request.Page < totalPages,
            HasPreviousPage = request.Page > 1
        };

        return new PagedResultDto<ProductResponseDto>
        {
            Data = data,
            Pagination = pagination
        };
    }
}