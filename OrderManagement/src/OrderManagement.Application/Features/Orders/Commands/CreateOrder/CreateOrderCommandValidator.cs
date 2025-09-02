using FluentValidation;
using OrderManagement.Application.Features.Orders.Commands.CreateOrder;

namespace OrderManagement.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one order item is required")
            .Must(HaveUniqueProducts).WithMessage("Duplicate products are not allowed");

        RuleForEach(x => x.Items)
            .SetValidator(new OrderItemDtoValidator());

        RuleFor(x => x.ShippingAddress)
            .MaximumLength(500).WithMessage("Shipping address must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.ShippingAddress));

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }

    private static bool HaveUniqueProducts(List<OrderItemDto> items)
    {
        if (items == null) return true;

        var productIds = items.Select(x => x.ProductId).ToList();
        return productIds.Count == productIds.Distinct().Count();
    }
}

public class OrderItemDtoValidator : AbstractValidator<OrderItemDto>
{
    public OrderItemDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(1000).WithMessage("Quantity must not exceed 1000");
    }
}