using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductManagement.Dtos;
using ProductManagement.Infrastructure.Functions;
using ProductManagement.Models.Validation;
using ProductManagement.Services.Interfaces;

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