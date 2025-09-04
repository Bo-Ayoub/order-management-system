using MediatR;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Application.Common.Models;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderCommandHandler(
            IRepository<Order> orderRepository,
            IRepository<Customer> customerRepository,
            IRepository<Product> productRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                // Get customer
                var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
                if (customer == null)
                    return Result<Guid>.Failure("Customer not found");

                // Create order
                var order = new Order(customer, request.ShippingAddress, request.Notes);

                // Add order items
                foreach (var item in request.Items)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
                    if (product == null)
                        return Result<Guid>.Failure($"Product with ID {item.ProductId} not found");

                    if (!product.IsInStock(item.Quantity))
                        return Result<Guid>.Failure($"Insufficient stock for product {product.Name}");

                    order.AddOrderItem(product, item.Quantity);

                    // Update product stock
                    product.UpdateStock(product.StockQuantity - item.Quantity);
                    await _productRepository.UpdateAsync(product, cancellationToken);
                }

                await _orderRepository.AddAsync(order, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<Guid>.Success(order.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<Guid>.Failure($"Failed to create order: {ex.Message}");
            }

            //try
            //{
            //    return await _unitOfWork.ExecuteTransactionAsync(async () =>
            //    {
            //        // Get customer
            //        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
            //        if (customer == null)
            //            return Result<Guid>.Failure("Customer not found");

            //        // Create order
            //        var order = new Order(customer, request.ShippingAddress, request.Notes);

            //        // Add order items
            //        foreach (var item in request.Items)
            //        {
            //            var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            //            if (product == null)
            //                return Result<Guid>.Failure($"Product with ID {item.ProductId} not found");

            //            if (!product.IsInStock(item.Quantity))
            //                return Result<Guid>.Failure($"Insufficient stock for product {product.Name}");

            //            order.AddOrderItem(product, item.Quantity);

            //            // Update product stock
            //            product.UpdateStock(product.StockQuantity - item.Quantity);
            //            await _productRepository.UpdateAsync(product, cancellationToken);
            //        }

            //        await _orderRepository.AddAsync(order, cancellationToken);
            //        return Result<Guid>.Success(order.Id);
            //    }, cancellationToken);
            //}
            //catch (Exception ex)
            //{
            //    return Result<Guid>.Failure($"Failed to create order: {ex.Message}");
            //}
        }
    }
}
