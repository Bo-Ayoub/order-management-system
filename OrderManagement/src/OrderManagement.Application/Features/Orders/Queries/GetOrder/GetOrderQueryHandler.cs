using MediatR;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Application.Common.Models;
using OrderManagement.Application.Features.Orders.Specifications;
using OrderManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Features.Orders.Queries.GetOrder
{
    public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, Result<OrderDto>>
    {
        private readonly IRepository<Order> _orderRepository;

        public GetOrderQueryHandler(IRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Result<OrderDto>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.FindOneAsync(
                new OrderByIdWithDetailsSpecification(request.OrderId),
                cancellationToken);

            if (order == null)
                return Result<OrderDto>.Failure("Order not found");

            var orderItemDtos = order.OrderItems.Select(item => new OrderItemDto(
                item.Id,
                item.ProductId,
                item.Product.Name,
                item.Quantity,
                item.UnitPrice.Amount,
                item.TotalPrice.Amount,
                item.UnitPrice.Currency
            )).ToList();

            var orderDto = new OrderDto(
                order.Id,
                order.OrderNumber,
                order.CustomerId,
                order.Customer.FullName,
                order.Status,
                order.OrderDate,
                order.ShippedDate,
                order.DeliveredDate,
                order.ShippingAddress,
                order.Notes,
                order.TotalAmount.Amount,
                order.TotalAmount.Currency,
                order.TotalItems,
                orderItemDtos
            );

            return Result<OrderDto>.Success(orderDto);
        }
    }
}
