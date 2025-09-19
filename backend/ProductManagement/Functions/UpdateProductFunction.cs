using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductManagement.Dtos;
using ProductManagement.Infrastructure.Functions;
using ProductManagement.Models.Validation;
using ProductManagement.Services.Interfaces;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;

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
    [OpenApiOperation(operationId: "UpdateProduct", tags: new[] { "Products" }, Summary = "Update product by ID", Description = "Updates an existing product with new information")]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "Product ID", Description = "The unique identifier of the product to update")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UpdateProductRequestDto), Required = true, Description = "Product update request")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ProductResponseDto), Summary = "Product updated successfully", Description = "Returns the updated product")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid request", Description = "The request body is invalid or missing required fields")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Product not found", Description = "No product found with the specified ID")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Summary = "Internal server error", Description = "An error occurred while updating the product")]
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