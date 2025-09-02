using OrderManagement.Application.Common.Models;
using Xunit;

namespace OrderManagement.UnitTests.Application.Common.Models;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Error);
    }

    [Fact]
    public void Failure_ShouldCreateFailedResult()
    {
        // Arrange
        var errorMessage = "Test error";

        // Act
        var result = Result.Failure(errorMessage);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(errorMessage, result.Error);
    }

    [Fact]
    public void GenericSuccess_ShouldCreateSuccessfulResultWithValue()
    {
        // Arrange
        var value = 42;

        // Act
        var result = Result<int>.Success(value);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value);
        Assert.Empty(result.Error);
    }

    [Fact]
    public void GenericFailure_ShouldCreateFailedResultWithoutValue()
    {
        // Arrange
        var errorMessage = "Test error";

        // Act
        var result = Result<int>.Failure(errorMessage);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(default(int), result.Value);
        Assert.Equal(errorMessage, result.Error);
    }

    [Fact]
    public void ImplicitConversion_FromString_ShouldCreateFailedResult()
    {
        // Arrange
        var errorMessage = "Implicit error";

        // Act
        Result result = Result.Failure(errorMessage);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(errorMessage, result.Error);
    }

    [Fact]
    public void ImplicitConversion_FromValue_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var value = "test value";

        // Act
        Result<string> result = value;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value);
    }
}
