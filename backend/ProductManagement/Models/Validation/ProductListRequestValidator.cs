using FluentValidation;
using ProductManagement.Dtos;

namespace ProductManagement.Models.Validation;

public class ProductListRequestValidator : AbstractValidator<ProductListRequestDto>
{
    private readonly string[] _allowedSortFields = { "name", "price", "createdat", "brand", "category" };

    public ProductListRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than or equal to 1");

        RuleFor(x => x.PerPage)
            .GreaterThanOrEqualTo(1)
            .WithMessage("PerPage must be greater than or equal to 1")
            .LessThanOrEqualTo(100)
            .WithMessage("PerPage cannot exceed 100");

        RuleFor(x => x.SortBy)
            .Must(BeValidSortField)
            .When(x => !string.IsNullOrEmpty(x.SortBy))
            .WithMessage($"SortBy must be one of: {string.Join(", ", _allowedSortFields)}");

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinPrice.HasValue)
            .WithMessage("MinPrice must be greater than or equal to 0");

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxPrice.HasValue)
            .WithMessage("MaxPrice must be greater than or equal to 0");

        RuleFor(x => x)
            .Must(x => !x.MinPrice.HasValue || !x.MaxPrice.HasValue || x.MinPrice <= x.MaxPrice)
            .WithMessage("MinPrice must be less than or equal to MaxPrice")
            .When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue);
    }

    private bool BeValidSortField(string? sortBy)
    {
        if (string.IsNullOrEmpty(sortBy))
            return true;

        return _allowedSortFields.Contains(sortBy.ToLower());
    }
}