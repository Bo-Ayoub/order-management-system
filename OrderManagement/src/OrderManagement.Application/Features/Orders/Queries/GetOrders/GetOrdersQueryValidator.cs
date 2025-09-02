using FluentValidation;
using OrderManagement.Application.Features.Orders.Queries.GetOrders;

namespace OrderManagement.Application.Features.Orders.Queries.GetOrders;

public class GetOrdersQueryValidator : AbstractValidator<GetOrdersQuery>
{
    public GetOrdersQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("Page size must be at least 1")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100");

        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID cannot be empty")
            .When(x => x.CustomerId.HasValue);

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid order status")
            .When(x => x.Status.HasValue);

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("From date cannot be in the future")
            .When(x => x.FromDate.HasValue);

        RuleFor(x => x.ToDate)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("To date cannot be in the future")
            .When(x => x.ToDate.HasValue);

        RuleFor(x => x)
            .Must(x => !x.FromDate.HasValue || !x.ToDate.HasValue || x.FromDate <= x.ToDate)
            .WithMessage("From date must be less than or equal to to date")
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue);
    }
}