using Moq;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Application.Features.Orders.Commands.ConfirmOrder;
using OrderManagement.Application.Features.Orders.Specifications;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.Domain.ValueObjects;
using Xunit;

namespace OrderManagement.UnitTests.Application.Features.Orders.Commands;

public class ConfirmOrderCommandHandlerTests
{
    private readonly Mock<IRepository<Order>> _orderRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly ConfirmOrderCommandHandler _handler;

    public ConfirmOrderCommandHandlerTests()
    {
        _orderRepository = new Mock<IRepository<Order>>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _handler = new ConfirmOrderCommandHandler(_orderRepository.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_WithValidOrder_ShouldConfirmSuccessfully()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var customer = new Customer("John", "Doe", new Email("john@example.com"));
        var product = new Product("Test Product", new Money(99.99m, "USD"), 10);
        var order = new Order(customer, "123 Test St");
        order.AddOrderItem(product, 1);

        var command = new ConfirmOrderCommand(orderId);

        _orderRepository.Setup(r => r.FindOneAsync(It.IsAny<OrderByIdWithItemsSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.Confirmed, order.Status);
        _orderRepository.Verify(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentOrder_ShouldReturnFailure()
    {
        // Arrange
        var command = new ConfirmOrderCommand(Guid.NewGuid());

        _orderRepository.Setup(r => r.FindOneAsync(It.IsAny<OrderByIdWithItemsSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Order not found", result.Error);
    }

    [Fact]
    public async Task Handle_WithEmptyOrder_ShouldReturnFailure()
    {
        // Arrange
        var customer = new Customer("John", "Doe", new Email("john@example.com"));
        var order = new Order(customer, "123 Test St"); // Empty order
        var command = new ConfirmOrderCommand(Guid.NewGuid());

        _orderRepository.Setup(r => r.FindOneAsync(It.IsAny<OrderByIdWithItemsSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Order must have at least one item", result.Error);
    }
}