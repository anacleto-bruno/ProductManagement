using FluentValidation;
using ProductManagement.Dtos;

namespace ProductManagement.Models.Validation;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequestDto>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Product name is required")
            .MaximumLength(200)
            .WithMessage("Product name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Product description is required")
            .MaximumLength(1000)
            .WithMessage("Product description cannot exceed 1000 characters");

        RuleFor(x => x.Brand)
            .NotEmpty()
            .WithMessage("Brand is required")
            .MaximumLength(100)
            .WithMessage("Brand cannot exceed 100 characters");

        RuleFor(x => x.Sku)
            .NotEmpty()
            .WithMessage("SKU is required")
            .MaximumLength(50)
            .WithMessage("SKU cannot exceed 50 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0")
            .LessThan(1000000)
            .WithMessage("Price cannot exceed 1,000,000");

        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage("Category is required")
            .MaximumLength(100)
            .WithMessage("Category cannot exceed 100 characters");
    }
}