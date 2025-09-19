using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductManagement.Dtos;
using ProductManagement.Infrastructure.Functions;
using ProductManagement.Models.Validation;
using ProductManagement.Services.Interfaces;

namespace ProductManagement.Functions;

public class UpdateProductFunction : BaseFunctionWithValidation<UpdateProductRequestDto, UpdateProductRequestValidator>
{
    private readonly IProductService _productService;

    public UpdateProductFunction(
        ILogger<UpdateProductFunction> logger,
        IProductService productService,
        UpdateProductRequestValidator validator)
        : base(logger, validator)
    {
        _productService = productService;
    }

    [Function("UpdateProduct")]
    public async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "products/{id:int}")] HttpRequestData req,
        int id)
    {
        _logger.LogInformation("Updating product with ID: {ProductId}", id);

        return await ExecuteWithValidationAsync<ProductResponseDto>(req, async updateRequest =>
        {
            _logger.LogInformation("Updating product {ProductId} with name: {ProductName}", id, updateRequest.Name);

            var result = await _productService.UpdateProductAsync(id, updateRequest);

            if (!result.IsSuccess)
            {
                if (result.ErrorMessage == "Product not found")
                {
                    throw new InvalidOperationException("Product not found");
                }
                throw new InvalidOperationException(result.ErrorMessage ?? "Failed to update product");
            }

            _logger.LogInformation("Product updated successfully: {ProductName}", result.Data?.Name);
            return result.Data!;
        });
    }
}