using OrderManagement.Application.Common.Specifications;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Features.Orders.Specifications;

public class OrderByIdWithItemsSpecification : BaseSpecification<Order>
{
    public OrderByIdWithItemsSpecification(Guid orderId)
        : base(o => o.Id == orderId)
    {
        AddInclude(o => o.OrderItems);
        AddInclude("OrderItems.Product"); // String-based include for nested navigation
    }
}