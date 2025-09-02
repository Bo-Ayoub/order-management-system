using OrderManagement.Domain.Enums;
using OrderManagement.Domain.Events;
using Xunit;

namespace OrderManagement.UnitTests.Domain.Events;

public class DomainEventTests
{
    [Fact]
    public void OrderCreatedEvent_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();

        // Act
        var domainEvent = new OrderCreatedEvent(orderId, customerId);

        // Assert
        Assert.Equal(orderId, domainEvent.OrderId);
        Assert.Equal(customerId, domainEvent.CustomerId);
        Assert.True(domainEvent.OccurredOn <= DateTime.UtcNow);
        Assert.True(domainEvent.OccurredOn > DateTime.UtcNow.AddSeconds(-1));
    }

    [Fact]
    public void OrderStatusChangedEvent_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var previousStatus = OrderStatus.Pending;
        var newStatus = OrderStatus.Confirmed;

        // Act
        var domainEvent = new OrderStatusChangedEvent(orderId, previousStatus, newStatus);

        // Assert
        Assert.Equal(orderId, domainEvent.OrderId);
        Assert.Equal(previousStatus, domainEvent.PreviousStatus);
        Assert.Equal(newStatus, domainEvent.NewStatus);
        Assert.True(domainEvent.OccurredOn <= DateTime.UtcNow);
        Assert.True(domainEvent.OccurredOn > DateTime.UtcNow.AddSeconds(-1));
    }
}
