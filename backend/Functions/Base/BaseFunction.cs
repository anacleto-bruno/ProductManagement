using FluentValidation;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductManagement.helpers;
using System.Net;

namespace ProductManagement.functions.@base;

public abstract class BaseFunction
{
    protected readonly ILogger _logger;

    protected BaseFunction(ILogger logger)
    {
        _logger = logger;
    }

    protected async Task<HttpResponseData> ExecuteSafelyAsync<TResponse>(
        HttpRequestData request, Func<Task<TResponse>> handler)
    {
        try
        {
            var result = await handler();
            return await HttpResponseHelper.CreateJsonResponseAsync(request, result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error in {FunctionName}", GetType().Name);
            return await HttpResponseHelper.CreateErrorResponseAsync(request, ex.Message, HttpStatusCode.BadRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {FunctionName}", GetType().Name);
            return await HttpResponseHelper.CreateErrorResponseAsync(request, "An unexpected error occurred", HttpStatusCode.InternalServerError);
        }
    }

    protected async Task<HttpResponseData> ExecuteSafelyAsync(
        HttpRequestData request, Func<Task<HttpResponseData>> handler)
    {
        try
        {
            return await handler();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error in {FunctionName}", GetType().Name);
            return await HttpResponseHelper.CreateErrorResponseAsync(request, ex.Message, HttpStatusCode.BadRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {FunctionName}", GetType().Name);
            return await HttpResponseHelper.CreateErrorResponseAsync(request, "An unexpected error occurred", HttpStatusCode.InternalServerError);
        }
    }
}

public abstract class BaseFunctionWithValidation<TRequest, TValidator> : BaseFunction
    where TRequest : new()
    where TValidator : AbstractValidator<TRequest>, new()
{
    private readonly TValidator _validator;

    protected BaseFunctionWithValidation(ILogger logger, TValidator validator) : base(logger)
    {
        _validator = validator;
    }

    protected async Task<HttpResponseData> ExecuteWithValidationAsync<TResponse>(
        HttpRequestData request, Func<TRequest, Task<TResponse>> handler)
    {
        try
        {
            var requestData = await RequestHelper.ParseJsonBodyAsync<TRequest>(request);
            
            var validationResult = await _validator.ValidateAsync(requestData);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return await HttpResponseHelper.CreateValidationErrorResponseAsync(request, errors);
            }

            var result = await handler(requestData);
            return await HttpResponseHelper.CreateJsonResponseAsync(request, result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error in {FunctionName}", GetType().Name);
            return await HttpResponseHelper.CreateErrorResponseAsync(request, ex.Message, HttpStatusCode.BadRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {FunctionName}", GetType().Name);
            return await HttpResponseHelper.CreateErrorResponseAsync(request, "An unexpected error occurred", HttpStatusCode.InternalServerError);
        }
    }
}