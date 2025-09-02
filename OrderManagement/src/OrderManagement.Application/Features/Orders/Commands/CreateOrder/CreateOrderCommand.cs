using MediatR;
using OrderManagement.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Features.Orders.Commands.CreateOrder
{
    public record CreateOrderCommand(
    Guid CustomerId,
    List<OrderItemDto> Items,
    string? ShippingAddress = null,
    string? Notes = null
    ) : IRequest<Result<Guid>>;

    public record OrderItemDto(
        Guid ProductId,
        int Quantity
    );
}
