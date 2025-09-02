using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.Domain.Events;
using OrderManagement.Domain.ValueObjects;
using Xunit;

namespace OrderManagement.UnitTests.Domain.Entities;

public class OrderTests
{
    private readonly Customer _testCustomer;
    private readonly Product _testProduct;

    public OrderTests()
    {
        _testCustomer = new Customer("John", "Doe", new Email("john@example.com"));
        _testProduct = new Product("Test Product", new Money(99.99m), 10);
    }

    [Fact]
    public void CreateOrder_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var order = new Order(_testCustomer, "123 Test St", "Test notes");

        // Assert
        Assert.Equal(_testCustomer.Id, order.CustomerId);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal("123 Test St", order.ShippingAddress);
        Assert.Equal("Test notes", order.Notes);
        Assert.NotEmpty(order.OrderNumber);
        Assert.Single(order.DomainEvents.OfType<OrderCreatedEvent>());
    }

    [Fact]
    public void AddOrderItem_ShouldAddItemCorrectly()
    {
        // Arrange
        var order = new Order(_testCustomer);

        // Act
        order.AddOrderItem(_testProduct, 2);

        // Assert
        Assert.Single(order.OrderItems);
        var orderItem = order.OrderItems.First();
        Assert.Equal(_testProduct.Id, orderItem.ProductId);
        Assert.Equal(2, orderItem.Quantity);
        Assert.Equal(_testProduct.Price, orderItem.UnitPrice);
    }

    [Fact]
    public void AddOrderItem_WhenProductOutOfStock_ShouldThrowException()
    {
        // Arrange
        var order = new Order(_testCustomer);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.AddOrderItem(_testProduct, 15));
    }

    [Fact]
    public void Confirm_WhenOrderHasItemsAndAddress_ShouldConfirmSuccessfully()
    {
        // Arrange
        var order = new Order(_testCustomer, "123 Test St");
        order.AddOrderItem(_testProduct, 2);

        // Act
        order.Confirm();

        // Assert
        Assert.Equal(OrderStatus.Confirmed, order.Status);
        Assert.Contains(order.DomainEvents, e => e is OrderStatusChangedEvent);
    }

    [Fact]
    public void Confirm_WhenOrderHasNoItems_ShouldThrowException()
    {
        // Arrange
        var order = new Order(_testCustomer, "123 Test St");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.Confirm());
    }

    [Fact]
    public void Cancel_WhenOrderNotDelivered_ShouldCancelSuccessfully()
    {
        // Arrange
        var order = new Order(_testCustomer);
        order.AddOrderItem(_testProduct, 1);

        // Act
        order.Cancel();

        // Assert
        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_WhenOrderDelivered_ShouldThrowException()
    {
        // Arrange
        var order = new Order(_testCustomer, "123 Test St");
        order.AddOrderItem(_testProduct, 1);
        order.Confirm();
        order.StartProcessing();
        order.Ship();
        order.Deliver();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.Cancel());
    }

    [Fact]
    public void TotalAmount_ShouldCalculateCorrectly()
    {
        // Arrange
        var order = new Order(_testCustomer);
        var product1 = new Product("Product 1", new Money(10.50m), 10);
        var product2 = new Product("Product 2", new Money(25.75m), 10);

        // Act
        order.AddOrderItem(product1, 2); // 21.00
        order.AddOrderItem(product2, 1); // 25.75

        // Assert
        Assert.Equal(46.75m, order.TotalAmount.Amount);
        Assert.Equal("USD", order.TotalAmount.Currency);
        Assert.Equal(3, order.TotalItems);
    }
}
