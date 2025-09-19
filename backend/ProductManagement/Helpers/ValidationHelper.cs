using FluentValidation;
using ProductManagement.Dtos;

namespace ProductManagement.Helpers;

public static class ValidationHelper
{
    public static ValidationErrorResponseDto CreateValidationErrorResponse(string validationErrors)
    {
        return new ValidationErrorResponseDto
        {
            Message = "Validation failed",
            StatusCode = 422,
            Details = validationErrors,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ValidationErrorResponseDto CreateValidationErrorResponse(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
    {
        var errors = failures
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray()
            );

        return new ValidationErrorResponseDto
        {
            Message = "Validation failed",
            StatusCode = 422,
            Errors = errors,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ErrorResponseDto CreateErrorResponse(string message, int statusCode, string? details = null)
    {
        return new ErrorResponseDto
        {
            Message = message,
            StatusCode = statusCode,
            Details = details,
            Timestamp = DateTime.UtcNow
        };
    }
}