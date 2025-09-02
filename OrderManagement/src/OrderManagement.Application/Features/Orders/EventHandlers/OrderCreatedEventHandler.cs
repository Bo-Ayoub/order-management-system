using MediatR;
using Microsoft.Extensions.Logging;
using OrderManagement.Domain.Events;

namespace OrderManagement.Application.Features.Orders.EventHandlers;

public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order {OrderId} has been created for customer {CustomerId}",
            notification.OrderId, notification.CustomerId);

        // Here you could add additional business logic such as:
        // - Send welcome email to customer
        // - Create audit log entry  
        // - Trigger inventory reservation
        // - Send notification to warehouse
        // - Update customer statistics

        await Task.CompletedTask;
    }
}