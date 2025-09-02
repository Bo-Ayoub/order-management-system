using MediatR;
using Microsoft.Extensions.Logging;
using OrderManagement.Domain.Events;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Features.Orders.EventHandlers;

public class OrderStatusChangedEventHandler : INotificationHandler<OrderStatusChangedEvent>
{
    private readonly ILogger<OrderStatusChangedEventHandler> _logger;

    public OrderStatusChangedEventHandler(ILogger<OrderStatusChangedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(OrderStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order {OrderId} status changed from {PreviousStatus} to {NewStatus}",
            notification.OrderId, notification.PreviousStatus, notification.NewStatus);

        // Handle different status transitions
        switch (notification.NewStatus)
        {
            case OrderStatus.Confirmed:
                await HandleOrderConfirmed(notification, cancellationToken);
                break;

            case OrderStatus.Processing:
                await HandleOrderProcessing(notification, cancellationToken);
                break;

            case OrderStatus.Shipped:
                await HandleOrderShipped(notification, cancellationToken);
                break;

            case OrderStatus.Delivered:
                await HandleOrderDelivered(notification, cancellationToken);
                break;

            case OrderStatus.Cancelled:
                await HandleOrderCancelled(notification, cancellationToken);
                break;
        }
    }

    private async Task HandleOrderConfirmed(OrderStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing confirmed order {OrderId}", notification.OrderId);

        await Task.CompletedTask;
    }

    private async Task HandleOrderProcessing(OrderStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order {OrderId} is now being processed", notification.OrderId);

        await Task.CompletedTask;
    }

    private async Task HandleOrderShipped(OrderStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order {OrderId} has been shipped", notification.OrderId);

        await Task.CompletedTask;
    }

    private async Task HandleOrderDelivered(OrderStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order {OrderId} has been delivered", notification.OrderId);


        await Task.CompletedTask;
    }

    private async Task HandleOrderCancelled(OrderStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order {OrderId} has been cancelled", notification.OrderId);

        await Task.CompletedTask;
    }
}
