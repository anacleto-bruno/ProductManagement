using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ProductManagement.dtos;
using ProductManagement.helpers;
using ProductManagement.models;
using ProductManagement.services;
using System.Net;

namespace ProductManagement.functions;

public class ProductFunctions
{
    private readonly ILogger<ProductFunctions> _logger;
    private readonly IProductService _productService;

    public ProductFunctions(
        ILogger<ProductFunctions> logger, 
        IProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    [Function("GetProduct")]
    [OpenApiOperation(operationId: "GetProduct", tags: new[] { "Products" }, Summary = "Get product by ID", Description = "Retrieves a single product by its unique identifier.")]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "The unique identifier of the product")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ProductResponseDto), Description = "Product found")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Product not found")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "Invalid product ID")]
    public async Task<HttpResponseData> GetByIdAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products/{id:int}")] HttpRequestData req,
        int id)
    {
        _logger.LogInformation("Getting product with ID: {ProductId}", id);

        if (id <= 0)
            return await FunctionResponseHelper.CreateBadRequestAsync(req, "Product ID must be greater than 0");

        var result = await _productService.GetByIdAsync(id);
        return await FunctionResponseHelper.CreateResponseAsync(req, result);
    }

    [Function("GetProducts")]
    [OpenApiOperation(operationId: "GetProducts", tags: new[] { "Products" }, Summary = "Get paginated products", Description = "Retrieves a paginated list of products with optional search and filtering.")]
    [OpenApiParameter(name: "page", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "Page number (default: 1)")]
    [OpenApiParameter(name: "pageSize", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "Items per page (default: 20)")]
    [OpenApiParameter(name: "searchTerm", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "Search term for name, description, or SKU")]
    [OpenApiParameter(name: "category", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "Filter by category")]
    [OpenApiParameter(name: "minPrice", In = ParameterLocation.Query, Required = false, Type = typeof(decimal), Description = "Minimum price filter")]
    [OpenApiParameter(name: "maxPrice", In = ParameterLocation.Query, Required = false, Type = typeof(decimal), Description = "Maximum price filter")]
    [OpenApiParameter(name: "sortBy", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "Sort field (name, price, createdAt)")]
    [OpenApiParameter(name: "descending", In = ParameterLocation.Query, Required = false, Type = typeof(bool), Description = "Sort in descending order")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PagedResultDto<ProductSummaryDto>), Description = "Paginated products list")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "Invalid pagination parameters")]
    public async Task<HttpResponseData> GetPagedAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products")] HttpRequestData req)
    {
        _logger.LogInformation("Getting paginated products");

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

        var result = await _productService.GetPagedAsync(paginationRequest);
        return await FunctionResponseHelper.CreateResponseAsync(req, result);
    }

    [Function("CreateProduct")]
    [OpenApiOperation(operationId: "CreateProduct", tags: new[] { "Products" }, Summary = "Create a new product", Description = "Creates a new product with the provided information.")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(CreateProductRequestDto), Required = true, Description = "Product creation data")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(ProductResponseDto), Description = "Product created successfully")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "Invalid product data or validation errors")]
    public async Task<HttpResponseData> CreateAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "products")] HttpRequestData req)
    {
        _logger.LogInformation("Creating new product");

        var createRequest = await RequestHelper.ParseJsonBodyAsync<CreateProductRequestDto>(req);
        
        var result = await _productService.CreateAsync(createRequest);
        
        if (result.IsSuccess)
            _logger.LogInformation("Product created successfully with ID: {ProductId}", result.Data!.Id);
        
        return await FunctionResponseHelper.CreateResponseAsync(req, result, HttpStatusCode.Created);
    }

    [Function("UpdateProduct")]
    [OpenApiOperation(operationId: "UpdateProduct", tags: new[] { "Products" }, Summary = "Update a product", Description = "Updates an existing product with the provided information.")]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "The unique identifier of the product to update")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UpdateProductRequestDto), Required = true, Description = "Product update data")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ProductResponseDto), Description = "Product updated successfully")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Product not found")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "Invalid product data or validation errors")]
    public async Task<HttpResponseData> UpdateAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "products/{id:int}")] HttpRequestData req,
        int id)
    {
        _logger.LogInformation("Updating product with ID: {ProductId}", id);

        if (id <= 0)
            return await FunctionResponseHelper.CreateBadRequestAsync(req, "Product ID must be greater than 0");

        var updateRequest = await RequestHelper.ParseJsonBodyAsync<UpdateProductRequestDto>(req);
        
        var result = await _productService.UpdateAsync(id, updateRequest);
        
        if (result.IsSuccess)
            _logger.LogInformation("Product updated successfully: {ProductId}", id);
        
        return await FunctionResponseHelper.CreateResponseAsync(req, result);
    }

    [Function("DeleteProduct")]
    [OpenApiOperation(operationId: "DeleteProduct", tags: new[] { "Products" }, Summary = "Delete a product", Description = "Deletes an existing product by its unique identifier.")]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "The unique identifier of the product to delete")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(object), Description = "Product deleted successfully")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Product not found")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "Invalid product ID")]
    public async Task<HttpResponseData> DeleteAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "products/{id:int}")] HttpRequestData req,
        int id)
    {
        _logger.LogInformation("Deleting product with ID: {ProductId}", id);

        if (id <= 0)
            return await FunctionResponseHelper.CreateBadRequestAsync(req, "Product ID must be greater than 0");

        var result = await _productService.DeleteAsync(id);
        
        if (result.IsSuccess)
            _logger.LogInformation("Product deleted successfully: {ProductId}", id);
        
        return await FunctionResponseHelper.CreateResponseAsync(req, result);
    }

    [Function("SeedProducts")]
    [OpenApiOperation(operationId: "SeedProducts", tags: new[] { "Products" }, Summary = "Seed database with mock products", Description = "Populates the database with a specified number of mock products for testing purposes.")]
    [OpenApiParameter(name: "count", In = ParameterLocation.Path, Required = false, Type = typeof(int), Description = "Number of products to create (1-10,000, default: 100)")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(object), Description = "Products seeded successfully")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "Invalid count parameter")]
    public async Task<HttpResponseData> SeedAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "products/seed/{count:int?}")] HttpRequestData req,
        int? count)
    {
        var seedCount = count ?? 100; // Default to 100 if not provided
        _logger.LogInformation("Seeding {Count} products", seedCount);

        if (seedCount <= 0 || seedCount > 10000)
            return await FunctionResponseHelper.CreateBadRequestAsync(req, "Count must be between 1 and 10,000");

        var result = await _productService.SeedAsync(seedCount);
        
        if (!result.IsSuccess)
        {
            _logger.LogError("Failed to seed products: {Error}", result.ErrorMessage);
            return await FunctionResponseHelper.CreateBadRequestAsync(req, result.ErrorMessage!);
        }

        _logger.LogInformation("Successfully seeded {Count} products", result.Data!.Count);
        return await FunctionResponseHelper.CreateCustomResponseAsync(req, HttpStatusCode.OK, new { 
            message = $"Successfully seeded {result.Data.Count} products",
            count = result.Data.Count,
            products = result.Data
        });
    }
}