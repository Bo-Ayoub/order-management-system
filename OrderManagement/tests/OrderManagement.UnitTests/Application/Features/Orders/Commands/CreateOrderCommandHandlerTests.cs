using Microsoft.Extensions.Logging;
using Moq;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Application.Features.Orders.Commands.CreateOrder;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.ValueObjects;
using Xunit;

namespace OrderManagement.UnitTests.Application.Features.Orders.Commands;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IRepository<Order>> _orderRepository;
    private readonly Mock<IRepository<Customer>> _customerRepository;
    private readonly Mock<IRepository<Product>> _productRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _orderRepository = new Mock<IRepository<Order>>();
        _customerRepository = new Mock<IRepository<Customer>>();
        _productRepository = new Mock<IRepository<Product>>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _handler = new CreateOrderCommandHandler(
            _orderRepository.Object,
            _customerRepository.Object,
            _productRepository.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateOrderSuccessfully()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var customer = new Customer("John", "Doe", new Email("john@example.com"));
        var product = new Product("Test Product", new Money(99.99m), 10);

        var command = new CreateOrderCommand(
            customerId,
            new List<OrderItemDto> { new(productId, 2) },
            "123 Test St",
            "Test order");

        _customerRepository.Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _productRepository.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);

        _unitOfWork.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _orderRepository.Verify(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidCustomer_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            new List<OrderItemDto> { new(Guid.NewGuid(), 1) });

        _customerRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Customer not found", result.Error);
    }

    [Fact]
    public async Task Handle_WithInsufficientStock_ShouldReturnFailure()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var customer = new Customer("John", "Doe", new Email("john@example.com"));
        var product = new Product("Test Product", new Money(99.99m), 1); // Only 1 in stock

        var command = new CreateOrderCommand(
            customerId,
            new List<OrderItemDto> { new(productId, 5) }); // Requesting 5

        _customerRepository.Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _productRepository.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Insufficient stock", result.Error);
    }
}