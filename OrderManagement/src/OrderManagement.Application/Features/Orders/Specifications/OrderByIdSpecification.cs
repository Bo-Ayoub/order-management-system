using OrderManagement.Application.Common.Specifications;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Features.Orders.Specifications;

public class OrderByIdSpecification : BaseSpecification<Order>
{
    public OrderByIdSpecification(Guid orderId)
        : base(o => o.Id == orderId)
    {
    }
}