using OrderManagement.Application.Features.Orders.Commands.CreateOrder;
using Xunit;

namespace OrderManagement.UnitTests.Application.Features.Orders.Commands;

public class CreateOrderCommandValidatorTests
{
    private readonly CreateOrderCommandValidator _validator;

    public CreateOrderCommandValidatorTests()
    {
        _validator = new CreateOrderCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldBeValid()
    {
        // Arrange
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            new List<OrderItemDto> { new(Guid.NewGuid(), 1) },
            "123 Test St");

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyCustomerId_ShouldBeInvalid()
    {
        // Arrange
        var command = new CreateOrderCommand(
            Guid.Empty,
            new List<OrderItemDto> { new(Guid.NewGuid(), 1) });

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateOrderCommand.CustomerId));
    }

    [Fact]
    public void Validate_WithEmptyItems_ShouldBeInvalid()
    {
        // Arrange
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            new List<OrderItemDto>());

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateOrderCommand.Items));
    }

    [Fact]
    public void Validate_WithDuplicateProducts_ShouldBeInvalid()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            new List<OrderItemDto>
            {
                new(productId, 1),
                new(productId, 2) // Duplicate product
            });

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Duplicate products"));
    }

    [Fact]
    public void Validate_WithInvalidQuantity_ShouldBeInvalid()
    {
        // Arrange
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            new List<OrderItemDto> { new(Guid.NewGuid(), 0) }); // Invalid quantity

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Quantity must be greater than 0"));
    }
}