using FluentValidation;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductManagement.Infrastructure.Http;
using System.Net;
using System.Text.Json;

namespace ProductManagement.Infrastructure.Functions;

public abstract class BaseFunctionWithValidation<TRequest, TValidator>
    where TRequest : new()
    where TValidator : AbstractValidator<TRequest>, new()
{
    protected readonly ILogger _logger;
    private readonly TValidator _validator;

    protected BaseFunctionWithValidation(ILogger logger, TValidator validator)
    {
        _logger = logger;
        _validator = validator;
    }

    protected async Task<HttpResponseData> ExecuteWithValidationAsync<TResponse>(
        HttpRequestData request,
        Func<TRequest, Task<TResponse>> handler)
    {
        try
        {
            // Parse request
            var requestData = await RequestHelper.ParseJsonBodyAsync<TRequest>(request);

            // Validate request
            var validationResult = await _validator.ValidateAsync(requestData);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Request validation failed: {Errors}",
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

                var errors = validationResult.Errors.Select(e =>
                    new ValidationError(e.PropertyName, e.ErrorMessage));

                return await HttpResponseHelper.CreateValidationErrorResponseAsync(request, errors);
            }

            // Execute handler
            var result = await handler(requestData);

            // Return success response
            return await HttpResponseHelper.CreateJsonResponseAsync(request, result);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Invalid JSON in request body");
            return await HttpResponseHelper.CreateErrorResponseAsync(
                request, "Invalid JSON format in request body", HttpStatusCode.BadRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during request processing");
            return await HttpResponseHelper.CreateErrorResponseAsync(
                request, "An unexpected error occurred", HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}