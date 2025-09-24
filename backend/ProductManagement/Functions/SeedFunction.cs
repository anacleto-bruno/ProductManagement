using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductManagement.Dtos;
using ProductManagement.Services.Interfaces;
using ProductManagement.Models.Validation;
using ProductManagement.Infrastructure.Functions;
using System.Net;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

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
    [OpenApiOperation(operationId: "SeedProducts", tags: new[] { "Data Management" }, Summary = "Seed database with sample products", Description = "Seeds the database with mock product data for testing and development purposes")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(SeedRequestDto), Required = true, Description = "Seed configuration request")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(SeedResponseDto), Summary = "Database seeded successfully", Description = "Returns statistics about the seeding operation")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ErrorResponseDto), Summary = "Invalid request", Description = "The request body is invalid or missing required fields")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Summary = "Internal server error", Description = "An error occurred during the seeding operation")]
    public async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "products/seed")] HttpRequestData req,
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