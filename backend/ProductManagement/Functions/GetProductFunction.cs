using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductManagement.Dtos;
using ProductManagement.Infrastructure.Functions;
using ProductManagement.Services.Interfaces;

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