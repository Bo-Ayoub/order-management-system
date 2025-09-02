using OrderManagement.Application.Common.Specifications;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Features.Orders.Specifications;

public class PendingOrdersSpecification : BaseSpecification<Order>
{
    public PendingOrdersSpecification()
        : base(o => o.Status == OrderStatus.Pending)
    {
        AddInclude(o => o.Customer);
        AddInclude(o => o.OrderItems);
        AddInclude("OrderItems.Product");
        ApplyOrderBy(o => o.OrderDate);
    }
}