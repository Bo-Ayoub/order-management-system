using OrderManagement.Domain.ValueObjects;
using Xunit;

namespace OrderManagement.UnitTests.Domain.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("user+tag@example.org")]
    public void CreateEmail_WithValidEmail_ShouldCreateCorrectly(string emailValue)
    {
        // Act
        var email = new Email(emailValue);

        // Assert
        Assert.Equal(emailValue.ToLowerInvariant(), email.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("test.example.com")]
    public void CreateEmail_WithInvalidEmail_ShouldThrowException(string invalidEmail)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Email(invalidEmail));
    }

    [Fact]
    public void Email_ShouldBeImplicitlyConvertibleToString()
    {
        // Arrange
        var email = new Email("test@example.com");

        // Act
        string emailString = email;

        // Assert
        Assert.Equal("test@example.com", emailString);
    }
}