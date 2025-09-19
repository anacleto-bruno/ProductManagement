using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductManagement.Dtos;
using ProductManagement.Infrastructure.Functions;
using ProductManagement.Models.Validation;
using ProductManagement.Services.Interfaces;
using System.Web;

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