using OrderManagement.Application.Common.Specifications;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Features.Orders.Specifications;

public class OrdersByCustomerSpecification : BaseSpecification<Order>
{
    public OrdersByCustomerSpecification(Guid customerId)
        : base(o => o.CustomerId == customerId)
    {
        AddInclude(o => o.OrderItems);
        AddInclude("OrderItems.Product");
        ApplyOrderByDescending(o => o.OrderDate);
    }
}