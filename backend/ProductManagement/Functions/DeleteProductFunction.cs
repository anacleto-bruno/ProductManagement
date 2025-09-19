using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductManagement.Infrastructure.Functions;
using ProductManagement.Services.Interfaces;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;

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
    [OpenApiOperation(operationId: "DeleteProduct", tags: new[] { "Products" }, Summary = "Delete product by ID", Description = "Deletes an existing product by its ID")]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "Product ID", Description = "The unique identifier of the product to delete")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(object), Summary = "Product deleted successfully", Description = "Confirms the product was deleted")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Product not found", Description = "No product found with the specified ID")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Summary = "Internal server error", Description = "An error occurred while deleting the product")]
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