using FluentValidation;
using ProductManagement.Dtos;

namespace ProductManagement.Models.Validation;

public class SeedRequestValidator : AbstractValidator<SeedRequestDto>
{
    public SeedRequestValidator()
    {
        RuleFor(x => x.NumRows)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Number of rows must be at least 1")
            .LessThanOrEqualTo(10000)
            .WithMessage("Number of rows cannot exceed 10,000 for performance reasons");
    }
}