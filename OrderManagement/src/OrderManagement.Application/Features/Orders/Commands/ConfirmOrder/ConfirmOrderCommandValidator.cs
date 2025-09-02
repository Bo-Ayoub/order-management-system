using FluentValidation;
using OrderManagement.Application.Features.Orders.Commands.ConfirmOrder;

namespace OrderManagement.Application.Features.Orders.Commands.ConfirmOrder;

public class ConfirmOrderCommandValidator : AbstractValidator<ConfirmOrderCommand>
{
    public ConfirmOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID is required");
    }
}