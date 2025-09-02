using Microsoft.Extensions.Logging;
using Moq;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Application.Features.Customers.Commands.CreateCustomer;
using OrderManagement.Application.Features.Customers.Specifications;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.ValueObjects;
using Xunit;

namespace OrderManagement.UnitTests.Application.Features.Customers.Commands;

public class CreateCustomerCommandHandlerTests
{
    private readonly Mock<IRepository<Customer>> _customerRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly CreateCustomerCommandHandler _handler;

    public CreateCustomerCommandHandlerTests()
    {
        _customerRepository = new Mock<IRepository<Customer>>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _handler = new CreateCustomerCommandHandler(_customerRepository.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateCustomerSuccessfully()
    {
        // Arrange
        var command = new CreateCustomerCommand("John", "Doe", "john@example.com", "+1-555-0123");

        _customerRepository.Setup(r => r.ExistsAsync(It.IsAny<CustomerByEmailSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
        _customerRepository.Verify(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateCustomerCommand("John", "Doe", "existing@example.com");

        _customerRepository.Setup(r => r.ExistsAsync(It.IsAny<CustomerByEmailSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("A customer with this email already exists", result.Error);
        _customerRepository.Verify(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidEmail_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateCustomerCommand("John", "Doe", "invalid-email");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Invalid email format", result.Error);
    }
}