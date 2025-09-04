using MediatR;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Application.Common.Models;
using OrderManagement.Application.Features.Orders.Specifications;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Result>
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateOrderStatusCommandHandler(
            IRepository<Order> orderRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var spec = new OrderByIdWithItemsSpecification(request.OrderId);
                var order = await _orderRepository.FindOneAsync(spec, cancellationToken);
                if (order == null)
                    return Result.Failure("Order not found");

                // Apply status transition based on target status
                switch (request.NewStatus)
                {
                    case OrderStatus.Confirmed:
                        order.Confirm();
                        break;
                    case OrderStatus.Processing:
                        order.StartProcessing();
                        break;
                    case OrderStatus.Shipped:
                        order.Ship();
                        break;
                    case OrderStatus.Delivered:
                        order.Deliver();
                        break;
                    case OrderStatus.Cancelled:
                        order.Cancel();
                        break;
                    default:
                        return Result.Failure($"Invalid status transition to {request.NewStatus}");
                }

                await _orderRepository.UpdateAsync(order, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to update order status: {ex.Message}");
            }
        }
    }
}
