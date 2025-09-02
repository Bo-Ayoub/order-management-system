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

namespace OrderManagement.Application.Features.Orders.Queries.GetOrders
{
    public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, Result<PaginatedList<OrderSummaryDto>>>
    {
        private readonly IRepository<Order> _orderRepository;

        public GetOrdersQueryHandler(IRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Result<PaginatedList<OrderSummaryDto>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            var specification = new OrderSearchSpecification(
                request.CustomerId,
                request.Status,
                request.FromDate,
                request.ToDate,
                request.PageNumber,
                request.PageSize);

            var orders = await _orderRepository.FindAsync(specification, cancellationToken);
            var totalCount = await _orderRepository.CountAsync(
                new OrderSearchSpecification(
                    request.CustomerId,
                    request.Status,
                    request.FromDate,
                    request.ToDate),
                cancellationToken);

            var orderDtos = orders.Select(o => new OrderSummaryDto(
                o.Id,
                o.OrderNumber,
                o.Customer.FullName,
                o.Status,
                o.OrderDate,
                o.TotalAmount.Amount,
                o.TotalAmount.Currency,
                o.TotalItems
            )).ToList();

            var paginatedList = PaginatedList<OrderSummaryDto>.Create(
                orderDtos,
                request.PageNumber,
                request.PageSize,
                totalCount);

            return Result<PaginatedList<OrderSummaryDto>>.Success(paginatedList);
        }
    }
}
