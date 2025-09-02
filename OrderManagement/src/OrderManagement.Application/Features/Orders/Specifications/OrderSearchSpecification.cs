using OrderManagement.Application.Common.Specifications;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using System.Linq.Expressions;

namespace OrderManagement.Application.Features.Orders.Specifications;

public class OrderSearchSpecification : BaseSpecification<Order>
{
    public OrderSearchSpecification(
        Guid? customerId = null,
        OrderStatus? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int? pageNumber = null,
        int? pageSize = null)
        : base(BuildCriteria(customerId, status, fromDate, toDate))
    {
        // Include customer and order items for summary data
        AddInclude(o => o.Customer);
        AddInclude(o => o.OrderItems);

        // Default ordering by order date (newest first)
        ApplyOrderByDescending(o => o.OrderDate);

        // Apply pagination if provided
        if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
        {
            ApplyPaging((pageNumber.Value - 1) * pageSize.Value, pageSize.Value);
        }
    }

    private static Expression<Func<Order, bool>>? BuildCriteria(
        Guid? customerId,
        OrderStatus? status,
        DateTime? fromDate,
        DateTime? toDate)
    {
        Expression<Func<Order, bool>>? criteria = null;

        // Customer filter
        if (customerId.HasValue)
        {
            Expression<Func<Order, bool>> customerCriteria = o => o.CustomerId == customerId.Value;
            criteria = CombineWithAnd(criteria, customerCriteria);
        }

        // Status filter
        if (status.HasValue)
        {
            Expression<Func<Order, bool>> statusCriteria = o => o.Status == status.Value;
            criteria = CombineWithAnd(criteria, statusCriteria);
        }

        // Date range filters
        if (fromDate.HasValue)
        {
            Expression<Func<Order, bool>> fromDateCriteria = o => o.OrderDate >= fromDate.Value;
            criteria = CombineWithAnd(criteria, fromDateCriteria);
        }

        if (toDate.HasValue)
        {
            Expression<Func<Order, bool>> toDateCriteria = o => o.OrderDate <= toDate.Value;
            criteria = CombineWithAnd(criteria, toDateCriteria);
        }

        return criteria;
    }

    private static Expression<Func<Order, bool>>? CombineWithAnd(
        Expression<Func<Order, bool>>? left,
        Expression<Func<Order, bool>> right)
    {
        if (left == null)
            return right;

        var parameter = Expression.Parameter(typeof(Order), "o");
        var leftInvoke = Expression.Invoke(left, parameter);
        var rightInvoke = Expression.Invoke(right, parameter);
        var andExpression = Expression.AndAlso(leftInvoke, rightInvoke);

        return Expression.Lambda<Func<Order, bool>>(andExpression, parameter);
    }
}