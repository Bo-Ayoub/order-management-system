using MediatR;
using OrderManagement.Application.Common.Models;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Features.Orders.Queries.GetOrder
{
    public record GetOrderQuery(Guid OrderId) : IRequest<Result<OrderDto>>;

    public record OrderDto(
        Guid Id,
        string OrderNumber,
        Guid CustomerId,
        string CustomerName,
        OrderStatus Status,
        DateTime OrderDate,
        DateTime? ShippedDate,
        DateTime? DeliveredDate,
        string? ShippingAddress,
        string? Notes,
        decimal TotalAmount,
        string Currency,
        int TotalItems,
        List<OrderItemDto> OrderItems
    );

    public record OrderItemDto(
        Guid Id,
        Guid ProductId,
        string ProductName,
        int Quantity,
        decimal UnitPrice,
        decimal TotalPrice,
        string Currency
    );
}
