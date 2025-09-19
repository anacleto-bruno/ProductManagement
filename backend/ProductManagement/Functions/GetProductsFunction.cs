using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductManagement.Dtos;
using ProductManagement.Infrastructure.Functions;
using ProductManagement.Models.Validation;
using ProductManagement.Services.Interfaces;
using System.Web;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;

namespace ProductManagement.Functions;

public class GetProductsFunction : BaseFunction
{
    private readonly IProductService _productService;
    private readonly ProductListRequestValidator _validator;

    public GetProductsFunction(
        ILogger<GetProductsFunction> logger,
        IProductService productService,
        ProductListRequestValidator validator)
        : base(logger)
    {
        _productService = productService;
        _validator = validator;
    }

    [Function("GetProducts")]
    [OpenApiOperation(operationId: "GetProducts", tags: new[] { "Products" }, Summary = "Get paginated products list", Description = "Retrieves a paginated list of products with optional filtering and sorting")]
    [OpenApiParameter(name: "page", In = ParameterLocation.Query, Required = false, Type = typeof(int), Summary = "Page number", Description = "Page number for pagination (default: 1)")]
    [OpenApiParameter(name: "per_page", In = ParameterLocation.Query, Required = false, Type = typeof(int), Summary = "Items per page", Description = "Number of items per page (default: 20, max: 100)")]
    [OpenApiParameter(name: "sort_by", In = ParameterLocation.Query, Required = false, Type = typeof(string), Summary = "Sort field", Description = "Field to sort by (name, price, createdat, brand, category)")]
    [OpenApiParameter(name: "descending", In = ParameterLocation.Query, Required = false, Type = typeof(bool), Summary = "Sort order", Description = "Sort in descending order (default: false)")]
    [OpenApiParameter(name: "search", In = ParameterLocation.Query, Required = false, Type = typeof(string), Summary = "Search term", Description = "Search term to filter products by name or description")]
    [OpenApiParameter(name: "category", In = ParameterLocation.Query, Required = false, Type = typeof(string), Summary = "Category filter", Description = "Filter products by category")]
    [OpenApiParameter(name: "brand", In = ParameterLocation.Query, Required = false, Type = typeof(string), Summary = "Brand filter", Description = "Filter products by brand")]
    [OpenApiParameter(name: "min_price", In = ParameterLocation.Query, Required = false, Type = typeof(decimal), Summary = "Minimum price", Description = "Filter products with price greater than or equal to this value")]
    [OpenApiParameter(name: "max_price", In = ParameterLocation.Query, Required = false, Type = typeof(decimal), Summary = "Maximum price", Description = "Filter products with price less than or equal to this value")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PagedResultDto<ProductResponseDto>), Summary = "Products retrieved successfully", Description = "Returns paginated list of products with metadata")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid parameters", Description = "Invalid query parameters provided")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Summary = "Internal server error", Description = "An error occurred while retrieving products")]
    public async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products")] HttpRequestData req)
    {
        _logger.LogInformation("Getting paginated product list");

        return await ExecuteSafelyAsync<PagedResultDto<ProductResponseDto>>(req, async () =>
        {
            // Parse query parameters
            var query = HttpUtility.ParseQueryString(req.Url.Query);

            var request = new ProductListRequestDto
            {
                Page = ParseIntParameter(query["page"], 1),
                PerPage = ParseIntParameter(query["per_page"], 20),
                SortBy = query["sort_by"],
                Descending = ParseBoolParameter(query["descending"], false),
                Search = query["search"],
                Category = query["category"],
                Brand = query["brand"],
                MinPrice = ParseDecimalParameter(query["min_price"]),
                MaxPrice = ParseDecimalParameter(query["max_price"])
            };

            // Validate request parameters
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException($"Invalid parameters: {errors}");
            }

            _logger.LogInformation("Retrieving products - Page: {Page}, PerPage: {PerPage}, SortBy: {SortBy}, Search: {Search}",
                request.Page, request.PerPage, request.SortBy, request.Search);

            var result = await _productService.GetProductsAsync(request);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException(result.ErrorMessage ?? "Failed to retrieve products");
            }

            _logger.LogInformation("Retrieved {Count} products out of {Total} total",
                result.Data?.Data.Count(), result.Data?.Pagination.TotalCount);

            return result.Data!;
        });
    }

    private static int ParseIntParameter(string? value, int defaultValue)
    {
        return int.TryParse(value, out var result) ? result : defaultValue;
    }

    private static bool ParseBoolParameter(string? value, bool defaultValue)
    {
        return bool.TryParse(value, out var result) ? result : defaultValue;
    }

    private static decimal? ParseDecimalParameter(string? value)
    {
        return decimal.TryParse(value, out var result) ? result : null;
    }
}