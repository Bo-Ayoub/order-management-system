using MediatR;
using OrderManagement.Application.Common.Models;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Features.Orders.Queries.GetOrders;

public record GetOrdersQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? CustomerId = null,
    OrderStatus? Status = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IRequest<Result<PaginatedList<OrderSummaryDto>>>;

public record OrderSummaryDto(
    Guid Id,
    string OrderNumber,
    string CustomerName,
    OrderStatus Status,
    DateTime OrderDate,
    decimal TotalAmount,
    string Currency,
    int TotalItems
);