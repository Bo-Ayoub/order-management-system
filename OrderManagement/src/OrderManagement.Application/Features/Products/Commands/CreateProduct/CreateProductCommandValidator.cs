using FluentValidation;
using OrderManagement.Application.Features.Products.Commands.CreateProduct;

namespace OrderManagement.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    private static readonly string[] ValidCurrencies = { "USD", "EUR", "GBP", "CAD", "AUD" };

    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters")
            .MinimumLength(2).WithMessage("Product name must be at least 2 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0")
            .LessThanOrEqualTo(1000000).WithMessage("Price must not exceed 1,000,000")
            .PrecisionScale(18, 2, true).WithMessage("Price can have at most 2 decimal places");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Must(BeValidCurrency).WithMessage($"Currency must be one of: {string.Join(", ", ValidCurrencies)}");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative")
            .LessThanOrEqualTo(1000000).WithMessage("Stock quantity must not exceed 1,000,000");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Category)
            .MaximumLength(100).WithMessage("Category must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Category));
    }

    private static bool BeValidCurrency(string currency)
    {
        return ValidCurrencies.Contains(currency?.ToUpperInvariant());
    }
}