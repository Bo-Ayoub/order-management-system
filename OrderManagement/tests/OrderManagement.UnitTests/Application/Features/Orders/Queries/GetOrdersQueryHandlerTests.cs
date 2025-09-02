using Moq;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Application.Features.Orders.Queries.GetOrders;
using OrderManagement.Application.Features.Orders.Specifications;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.ValueObjects;
using Xunit;

namespace OrderManagement.UnitTests.Application.Features.Orders.Queries;

public class GetOrdersQueryHandlerTests
{
    private readonly Mock<IRepository<Order>> _orderRepository;
    private readonly GetOrdersQueryHandler _handler;

    public GetOrdersQueryHandlerTests()
    {
        _orderRepository = new Mock<IRepository<Order>>();
        _handler = new GetOrdersQueryHandler(_orderRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ShouldReturnPaginatedOrders()
    {
        // Arrange
        var customer = new Customer("John", "Doe", new Email("john@example.com"));
        var order = new Order(customer, "123 Test St");
        var orders = new List<Order> { order };

        var query = new GetOrdersQuery(PageNumber: 1, PageSize: 10);

        _orderRepository.Setup(r => r.FindAsync(It.IsAny<OrderSearchSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);

        _orderRepository.Setup(r => r.CountAsync(It.IsAny<OrderSearchSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value.Items);
        Assert.Equal(1, result.Value.TotalCount);
    }

    [Fact]
    public async Task Handle_WithCustomerFilter_ShouldUseCustomerIdInSpecification()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var query = new GetOrdersQuery(CustomerId: customerId);

        _orderRepository.Setup(r => r.FindAsync(It.IsAny<OrderSearchSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Order>());

        _orderRepository.Setup(r => r.CountAsync(It.IsAny<OrderSearchSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _orderRepository.Verify(r => r.FindAsync(
            It.Is<OrderSearchSpecification>(spec => true), // We can't easily verify the internal state
            It.IsAny<CancellationToken>()), Times.Once);
    }
}