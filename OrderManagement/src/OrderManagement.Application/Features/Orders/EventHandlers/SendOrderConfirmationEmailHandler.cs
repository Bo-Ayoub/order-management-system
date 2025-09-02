using MediatR;
using Microsoft.Extensions.Logging;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Domain.Events;
using OrderManagement.Domain.Entities;
using OrderManagement.Application.Features.Orders.Specifications;

namespace OrderManagement.Application.Features.Orders.EventHandlers;

public class SendOrderConfirmationEmailHandler : INotificationHandler<OrderCreatedEvent>
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<SendOrderConfirmationEmailHandler> _logger;

    public SendOrderConfirmationEmailHandler(
        IRepository<Order> orderRepository,
        IEmailService emailService,
        ILogger<SendOrderConfirmationEmailHandler> logger)
    {
        _orderRepository = orderRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            // Get order with customer details
            var order = await _orderRepository.FindOneAsync(
                new OrderByIdWithDetailsSpecification(notification.OrderId),
                cancellationToken);

            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found for email confirmation", notification.OrderId);
                return;
            }

            // Send confirmation email
            await _emailService.SendOrderConfirmationEmailAsync(
                order.Customer.Email.Value,
                order.Customer.FullName,
                order.OrderNumber,
                order.TotalAmount.ToString(),
                cancellationToken);

            _logger.LogInformation("Order confirmation email sent for order {OrderId} to {Email}",
                notification.OrderId, order.Customer.Email.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send order confirmation email for order {OrderId}",
                notification.OrderId);

            // Don't rethrow - email failure shouldn't fail the order creation
            // In a real system, you might want to queue this for retry
        }
    }
}