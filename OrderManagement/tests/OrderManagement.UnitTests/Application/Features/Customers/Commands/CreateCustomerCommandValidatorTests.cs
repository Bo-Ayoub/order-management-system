using OrderManagement.Application.Features.Customers.Commands.CreateCustomer;
using Xunit;

namespace OrderManagement.UnitTests.Application.Features.Customers.Commands;

public class CreateCustomerCommandValidatorTests
{
    private readonly CreateCustomerCommandValidator _validator;

    public CreateCustomerCommandValidatorTests()
    {
        _validator = new CreateCustomerCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldBeValid()
    {
        // Arrange
        var command = new CreateCustomerCommand("John", "Doe", "john@example.com", "+1-555-0123");

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WithInvalidFirstName_ShouldBeInvalid(string firstName)
    {
        // Arrange
        var command = new CreateCustomerCommand(firstName, "Doe", "john@example.com");

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateCustomerCommand.FirstName));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WithInvalidLastName_ShouldBeInvalid(string lastName)
    {
        // Arrange
        var command = new CreateCustomerCommand("John", lastName, "john@example.com");

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateCustomerCommand.LastName));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    public void Validate_WithInvalidEmail_ShouldBeInvalid(string email)
    {
        // Arrange
        var command = new CreateCustomerCommand("John", "Doe", email);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateCustomerCommand.Email));
    }

    [Fact]
    public void Validate_WithTooLongName_ShouldBeInvalid()
    {
        // Arrange
        var longName = new string('A', 101); // 101 characters
        var command = new CreateCustomerCommand(longName, "Doe", "john@example.com");

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateCustomerCommand.FirstName));
    }

    [Fact]
    public void Validate_WithInvalidPhoneNumber_ShouldBeInvalid()
    {
        // Arrange
        var command = new CreateCustomerCommand("John", "Doe", "john@example.com", "invalid-phone");

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateCustomerCommand.PhoneNumber));
    }
}
