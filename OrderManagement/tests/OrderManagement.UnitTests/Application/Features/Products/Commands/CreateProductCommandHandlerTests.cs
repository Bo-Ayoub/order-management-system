using Moq;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Application.Features.Products.Commands.CreateProduct;
using OrderManagement.Domain.Entities;
using Xunit;

namespace OrderManagement.UnitTests.Application.Features.Products.Commands;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IRepository<Product>> _productRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _productRepository = new Mock<IRepository<Product>>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _handler = new CreateProductCommandHandler(_productRepository.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateProductSuccessfully()
    {
        // Arrange
        var command = new CreateProductCommand("Test Product", 99.99m, 50, "USD", "Description", "Category");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
        _productRepository.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNegativePrice_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateProductCommand("Test Product", -10m, 50);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Amount cannot be negative", result.Error);
    }

    [Fact]
    public async Task Handle_WithNegativeStock_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateProductCommand("Test Product", 99.99m, -5);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Stock quantity cannot be negative", result.Error);
    }
}