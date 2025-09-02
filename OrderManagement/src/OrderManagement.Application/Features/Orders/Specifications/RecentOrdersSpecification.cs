using OrderManagement.Application.Common.Specifications;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Features.Orders.Specifications;

public class RecentOrdersSpecification : BaseSpecification<Order>
{
    public RecentOrdersSpecification(int days = 7, int take = 10)
        : base(o => o.OrderDate >= DateTime.UtcNow.AddDays(-days))
    {
        AddInclude(o => o.Customer);
        AddInclude(o => o.OrderItems);
        ApplyOrderByDescending(o => o.OrderDate);
        ApplyPaging(0, take);
    }
}