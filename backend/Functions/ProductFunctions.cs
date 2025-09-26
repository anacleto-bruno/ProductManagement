using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductManagement.dtos;
using ProductManagement.functions.@base;
using ProductManagement.helpers;
using ProductManagement.models;
using ProductManagement.services;
using ProductManagement.validators;
using System.Net;

namespace ProductManagement.functions;

public class ProductFunctions : BaseFunctionWithValidation<CreateProductRequestDto, CreateProductRequestValidator>
{
    private readonly IProductService _productService;
    private readonly UpdateProductRequestValidator _updateValidator;
    private readonly PaginationRequestValidator _paginationValidator;

    public ProductFunctions(
        ILogger<ProductFunctions> logger, 
        IProductService productService,
        CreateProductRequestValidator createValidator,
        UpdateProductRequestValidator updateValidator,
        PaginationRequestValidator paginationValidator) 
        : base(logger, createValidator)
    {
        _productService = productService;
        _updateValidator = updateValidator;
        _paginationValidator = paginationValidator;
    }

    [Function("GetProduct")]
    public async Task<HttpResponseData> GetByIdAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products/{id:int}")] HttpRequestData req,
        int id)
    {
        return await ExecuteSafelyAsync(req, async () =>
        {
            var result = await _productService.GetByIdAsync(id);
            if (!result.IsSuccess)
            {
                throw new ArgumentException(result.ErrorMessage);
            }
            return result.Data;
        });
    }

    [Function("GetProducts")]
    public async Task<HttpResponseData> GetPagedAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products")] HttpRequestData req)
    {
        return await ExecuteSafelyAsync(req, async () =>
        {
            // Parse query parameters
            var query = RequestHelper.ParseQueryString(req.Url.Query);
            
            var paginationRequest = new PaginationRequestDto
            {
                Page = query.TryGetValue("page", out var pageStr) && int.TryParse(pageStr, out var page) ? page : 1,
                PageSize = query.TryGetValue("pageSize", out var pageSizeStr) && int.TryParse(pageSizeStr, out var pageSize) ? pageSize : 20,
                SearchTerm = query.TryGetValue("searchTerm", out var searchTerm) ? searchTerm : null,
                Category = query.TryGetValue("category", out var category) ? category : null,
                MinPrice = query.TryGetValue("minPrice", out var minPriceStr) && decimal.TryParse(minPriceStr, out var minPrice) ? minPrice : null,
                MaxPrice = query.TryGetValue("maxPrice", out var maxPriceStr) && decimal.TryParse(maxPriceStr, out var maxPrice) ? maxPrice : null,
                SortBy = query.TryGetValue("sortBy", out var sortBy) ? sortBy : null,
                Descending = query.TryGetValue("descending", out var descendingStr) && bool.TryParse(descendingStr, out var desc) && desc
            };

            // Validate pagination request
            var validationResult = await _paginationValidator.ValidateAsync(paginationRequest);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new ArgumentException(string.Join(", ", errors));
            }

            var result = await _productService.GetPagedAsync(paginationRequest);
            if (!result.IsSuccess)
            {
                throw new ArgumentException(result.ErrorMessage);
            }
            return result.Data;
        });
    }

    [Function("CreateProduct")]
    public async Task<HttpResponseData> CreateAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "products")] HttpRequestData req)
    {
        return await ExecuteWithValidationAsync<object>(req, async createRequest =>
        {
            var result = await _productService.CreateAsync(createRequest);
            if (!result.IsSuccess)
            {
                throw new ArgumentException(result.ErrorMessage);
            }
            return result.Data!;
        });
    }

    [Function("UpdateProduct")]
    public async Task<HttpResponseData> UpdateAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "products/{id:int}")] HttpRequestData req,
        int id)
    {
        return await ExecuteSafelyAsync(req, async () =>
        {
            var updateRequest = await RequestHelper.ParseJsonBodyAsync<UpdateProductRequestDto>(req);
            
            var validationResult = await _updateValidator.ValidateAsync(updateRequest);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new ArgumentException(string.Join(", ", errors));
            }

            var result = await _productService.UpdateAsync(id, updateRequest);
            if (!result.IsSuccess)
            {
                throw new ArgumentException(result.ErrorMessage);
            }

            return result.Data;
        });
    }

    [Function("DeleteProduct")]
    public async Task<HttpResponseData> DeleteAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "products/{id:int}")] HttpRequestData req,
        int id)
    {
        return await ExecuteSafelyAsync(req, async () =>
        {
            var result = await _productService.DeleteAsync(id);
            if (!result.IsSuccess)
            {
                throw new ArgumentException(result.ErrorMessage);
            }
            return new { message = "Product deleted successfully" };
        });
    }

    [Function("SeedProducts")]
    public async Task<HttpResponseData> SeedAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "products/seed/{count:int?}")] HttpRequestData req,
        int? count)
    {
        return await ExecuteSafelyAsync(req, async () =>
        {
            var seedCount = count ?? 100; // Default to 100 if not provided
            var result = await _productService.SeedAsync(seedCount);
            if (!result.IsSuccess)
            {
                throw new ArgumentException(result.ErrorMessage);
            }
            return new { 
                message = $"Successfully seeded {result.Data!.Count} products",
                count = result.Data.Count,
                products = result.Data
            };
        });
    }
}