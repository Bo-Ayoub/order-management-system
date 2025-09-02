using MediatR;
using OrderManagement.Application.Common.Models;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public record UpdateOrderStatusCommand(
     Guid OrderId,
     OrderStatus NewStatus
 ) : IRequest<Result>;
}
