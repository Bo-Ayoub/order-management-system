using MediatR;
using Microsoft.Extensions.Logging;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Domain.Events;
using OrderManagement.Domain.Entities;
using OrderManagement.Application.Features.Orders.Specifications;

namespace OrderManagement.Application.Features.Orders.EventHandlers;

public class UpdateInventoryOnOrderCreatedHandler : INotificationHandler<OrderCreatedEvent>
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateInventoryOnOrderCreatedHandler> _logger;

    public UpdateInventoryOnOrderCreatedHandler(
        IRepository<Order> orderRepository,
        IRepository<Product> productRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateInventoryOnOrderCreatedHandler> logger)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            // Get order with items
            var order = await _orderRepository.FindOneAsync(
                new OrderByIdWithItemsSpecification(notification.OrderId),
                cancellationToken);

            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found for inventory update", notification.OrderId);
                return;
            }

            // Note: In the CreateOrderCommandHandler, we already updated the stock
            // This handler could be used for additional inventory management:
            // - Create inventory reservations
            // - Log inventory movements
            // - Send low stock alerts

            foreach (var orderItem in order.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync(orderItem.ProductId, cancellationToken);
                if (product != null)
                {
                    _logger.LogInformation("Inventory updated for product {ProductId}: {Quantity} units reserved",
                        product.Id, orderItem.Quantity);

                    // Check if product is running low on stock
                    if (product.StockQuantity <= 10) // Low stock threshold
                    {
                        _logger.LogWarning("Low stock alert: Product {ProductName} ({ProductId}) has only {StockQuantity} units remaining",
                            product.Name, product.Id, product.StockQuantity);

                        // In a real system, you might:
                        // - Send alert to purchasing department
                        // - Automatically reorder from suppliers
                        // - Update product visibility on website
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Inventory processing completed for order {OrderId}", notification.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process inventory for order {OrderId}", notification.OrderId);
            // Don't rethrow - inventory logging failure shouldn't fail the order
        }
    }
}