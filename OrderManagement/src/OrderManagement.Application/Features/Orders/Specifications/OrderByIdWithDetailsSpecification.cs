using OrderManagement.Application.Common.Specifications;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Features.Orders.Specifications;

public class OrderByIdWithDetailsSpecification : BaseSpecification<Order>
{
    public OrderByIdWithDetailsSpecification(Guid orderId)
        : base(o => o.Id == orderId)
    {
        AddInclude(o => o.Customer);
        AddInclude(o => o.OrderItems);
        AddInclude("OrderItems.Product");
    }
}