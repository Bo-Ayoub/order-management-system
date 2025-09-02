using MediatR;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Application.Common.Models;
using OrderManagement.Application.Features.Orders.Specifications;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Features.Orders.Commands.ConfirmOrder
{
    public class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand, Result>
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmOrderCommandHandler(
            IRepository<Order> orderRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var order = await _orderRepository.FindOneAsync(
                    new OrderByIdWithItemsSpecification(request.OrderId),
                    cancellationToken);

                if (order == null)
                    return Result.Failure("Order not found");

                // Validate before confirmation
                order.ValidateForConfirmation();

                // Confirm the order
                order.Confirm();

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
                return Result.Failure($"Failed to confirm order: {ex.Message}");
            }
        }
    }
}
