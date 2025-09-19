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

public class CreateProductFunction : BaseFunctionWithValidation<CreateProductRequestDto, CreateProductRequestValidator>
{
    private readonly IProductService _productService;

    public CreateProductFunction(
        ILogger<CreateProductFunction> logger,
        IProductService productService,
        CreateProductRequestValidator validator)
        : base(logger, validator)
    {
        _productService = productService;
    }

    [Function("CreateProduct")]
    [OpenApiOperation(operationId: "CreateProduct", tags: new[] { "Products" }, Summary = "Create a new product", Description = "Creates a new product with the provided information")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(CreateProductRequestDto), Required = true, Description = "Product creation request")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ProductResponseDto), Summary = "Product created successfully", Description = "Returns the created product")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid request", Description = "The request body is invalid or missing required fields")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Summary = "Internal server error", Description = "An error occurred while creating the product")]
    public async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "products")] HttpRequestData req)
    {
        _logger.LogInformation("Creating new product");

        return await ExecuteWithValidationAsync<ProductResponseDto>(req, async createRequest =>
        {
            _logger.LogInformation("Creating product with name: {ProductName}", createRequest.Name);

            var result = await _productService.CreateProductAsync(createRequest);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException(result.ErrorMessage ?? "Failed to create product");
            }

            _logger.LogInformation("Product created successfully with ID: {ProductId}", result.Data?.Id);
            return result.Data!;
        });
    }
}