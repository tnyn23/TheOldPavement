using FluentValidation;
using TheOldPavement.Application.DTOs;

namespace TheOldPavement.Application.Validators;

public class OrderValidator : AbstractValidator<CreateOrderDTO>
{
    public OrderValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID is required.");

        RuleFor(x => x.ShippingAddress)
            .NotEmpty().WithMessage("Shipping address is required.")
            .MaximumLength(500).WithMessage("Shipping address must not exceed 500 characters.");

        RuleFor(x => x.PaymentMethod)
            .Must(m => new[] { "cod", "momo", "stripe" }.Contains(m))
            .WithMessage("Payment method must be cod, momo, or stripe.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must have at least one item.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).GreaterThan(0).WithMessage("Product ID is required.");
            item.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("Quantity must be at least 1.");
        });
    }
}
