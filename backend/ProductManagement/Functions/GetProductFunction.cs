using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductManagement.Dtos;
using ProductManagement.Infrastructure.Functions;
using ProductManagement.Services.Interfaces;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;

namespace ProductManagement.Functions;

public class GetProductFunction : BaseFunction
{
    private readonly IProductService _productService;

    public GetProductFunction(
        ILogger<GetProductFunction> logger,
        IProductService productService)
        : base(logger)
    {
        _productService = productService;
    }

    [Function("GetProduct")]
    [OpenApiOperation(operationId: "GetProduct", tags: new[] { "Products" }, Summary = "Get product by ID", Description = "Retrieves a specific product by its ID")]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "Product ID", Description = "The unique identifier of the product")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ProductResponseDto), Summary = "Product found", Description = "Returns the requested product")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Product not found", Description = "No product found with the specified ID")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Summary = "Internal server error", Description = "An error occurred while retrieving the product")]
    public async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products/{id:int}")] HttpRequestData req,
        int id)
    {
        _logger.LogInformation("Getting product with ID: {ProductId}", id);

        return await ExecuteSafelyAsync<ProductResponseDto>(req, async () =>
        {
            var result = await _productService.GetProductByIdAsync(id);

            if (!result.IsSuccess)
            {
                if (result.ErrorMessage == "Product not found")
                {
                    throw new InvalidOperationException("Product not found");
                }
                throw new InvalidOperationException(result.ErrorMessage ?? "Failed to retrieve product");
            }

            _logger.LogInformation("Product retrieved successfully: {ProductName}", result.Data?.Name);
            return result.Data!;
        });
    }
}