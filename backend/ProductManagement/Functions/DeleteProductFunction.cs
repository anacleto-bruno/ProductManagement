using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductManagement.Infrastructure.Functions;
using ProductManagement.Services.Interfaces;

namespace ProductManagement.Functions;

public class DeleteProductFunction : BaseFunction
{
    private readonly IProductService _productService;

    public DeleteProductFunction(
        ILogger<DeleteProductFunction> logger,
        IProductService productService)
        : base(logger)
    {
        _productService = productService;
    }

    [Function("DeleteProduct")]
    public async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "products/{id:int}")] HttpRequestData req,
        int id)
    {
        _logger.LogInformation("Deleting product with ID: {ProductId}", id);

        return await ExecuteSafelyAsync<object>(req, async () =>
        {
            var result = await _productService.DeleteProductAsync(id);

            if (!result.IsSuccess)
            {
                if (result.ErrorMessage == "Product not found")
                {
                    throw new InvalidOperationException("Product not found");
                }
                throw new InvalidOperationException(result.ErrorMessage ?? "Failed to delete product");
            }

            _logger.LogInformation("Product deleted successfully with ID: {ProductId}", id);
            return new { message = "Product deleted successfully" };
        });
    }
}