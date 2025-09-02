using FluentValidation;
using OrderManagement.Application.Features.Orders.Commands.UpdateOrderStatus;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID is required");

        RuleFor(x => x.NewStatus)
            .IsInEnum().WithMessage("Invalid order status")
            .NotEqual(OrderStatus.Pending).WithMessage("Cannot manually set status to Pending");
    }
}