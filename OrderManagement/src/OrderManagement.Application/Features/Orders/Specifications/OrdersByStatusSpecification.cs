using OrderManagement.Application.Common.Specifications;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Features.Orders.Specifications;

public class OrdersByStatusSpecification : BaseSpecification<Order>
{
    public OrdersByStatusSpecification(OrderStatus status)
        : base(o => o.Status == status)
    {
        AddInclude(o => o.Customer);
        AddInclude(o => o.OrderItems);
        ApplyOrderByDescending(o => o.OrderDate);
    }
}