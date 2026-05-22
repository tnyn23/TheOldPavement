using FluentValidation;
using TheOldPavement.Application.DTOs;

namespace TheOldPavement.Application.Validators;

public class ProductValidator : AbstractValidator<CreateProductDTO>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity must be 0 or more.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required.");

        RuleFor(x => x.Status)
            .Must(s => new[] { "available", "sold_out", "coming_soon" }.Contains(s))
            .WithMessage("Status must be available, sold_out, or coming_soon.");
    }
}
