using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductManagement.Dtos;
using ProductManagement.Services.Interfaces;
using ProductManagement.Models.Validation;
using ProductManagement.Infrastructure.Functions;
using System.Net;

namespace ProductManagement.Functions;

public class SeedFunction : BaseFunctionWithValidation<SeedRequestDto, SeedRequestValidator>
{
    private readonly ISeedDataService _seedDataService;

    public SeedFunction(
        ILogger<SeedFunction> logger,
        ISeedDataService seedDataService,
        SeedRequestValidator validator)
        : base(logger, validator)
    {
        _seedDataService = seedDataService;
    }

    [Function("SeedProducts")]
    public async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "products/seed")] HttpRequestData req,
        FunctionContext context)
    {
        _logger.LogInformation("Seed products endpoint called");

        return await ExecuteWithValidationAsync<SeedResponseDto>(req, async seedRequest =>
        {
            _logger.LogInformation("Starting database seeding with {NumRows} products", seedRequest.NumRows);

            var result = await _seedDataService.SeedDataAsync(seedRequest.NumRows);

            // Check if seeding was successful
            if (result.ProductsCreated == 0)
            {
                throw new InvalidOperationException("No products were created during seeding operation");
            }

            _logger.LogInformation("Seed operation completed. Products created: {ProductsCreated}, Colors: {ColorsCreated}, Sizes: {SizesCreated}",
                result.ProductsCreated, result.ColorsCreated, result.SizesCreated);

            return result;
        });
    }
}