using OrderManagement.Domain.ValueObjects;
using Xunit;

namespace OrderManagement.UnitTests.Domain.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void CreateMoney_WithValidValues_ShouldCreateCorrectly()
    {
        // Arrange & Act
        var money = new Money(100.50m, "USD");

        // Assert
        Assert.Equal(100.50m, money.Amount);
        Assert.Equal("USD", money.Currency);
    }

    [Fact]
    public void CreateMoney_WithNegativeAmount_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Money(-10, "USD"));
    }

    [Fact]
    public void Add_WithSameCurrency_ShouldAddCorrectly()
    {
        // Arrange
        var money1 = new Money(10.50m, "USD");
        var money2 = new Money(5.25m, "USD");

        // Act
        var result = money1.Add(money2);

        // Assert
        Assert.Equal(15.75m, result.Amount);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public void Add_WithDifferentCurrency_ShouldThrowException()
    {
        // Arrange
        var money1 = new Money(10, "USD");
        var money2 = new Money(5, "EUR");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => money1.Add(money2));
    }

    [Fact]
    public void Multiply_ShouldCalculateCorrectly()
    {
        // Arrange
        var money = new Money(10.50m, "USD");

        // Act
        var result = money.Multiply(3);

        // Assert
        Assert.Equal(31.50m, result.Amount);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var money1 = new Money(100, "USD");
        var money2 = new Money(100, "USD");

        // Act & Assert
        Assert.Equal(money1, money2);
        Assert.True(money1 == money2);
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldReturnFalse()
    {
        // Arrange
        var money1 = new Money(100, "USD");
        var money2 = new Money(100, "EUR");

        // Act & Assert
        Assert.NotEqual(money1, money2);
        Assert.True(money1 != money2);
    }
}