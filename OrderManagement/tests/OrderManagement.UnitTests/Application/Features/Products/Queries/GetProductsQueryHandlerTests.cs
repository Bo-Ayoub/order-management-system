using Moq;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Application.Features.Products.Queries.GetProducts;
using OrderManagement.Application.Features.Products.Specifications;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.ValueObjects;
using Xunit;

namespace OrderManagement.UnitTests.Application.Features.Products.Queries;

public class GetProductsQueryHandlerTests
{
    private readonly Mock<IRepository<Product>> _productRepository;
    private readonly GetProductsQueryHandler _handler;

    public GetProductsQueryHandlerTests()
    {
        _productRepository = new Mock<IRepository<Product>>();
        _handler = new GetProductsQueryHandler(_productRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ShouldReturnPaginatedProducts()
    {
        // Arrange
        var product = new Product("Test Product", new Money(99.99m, "USD"), 10);
        var products = new List<Product> { product };

        var query = new GetProductsQuery(PageNumber: 1, PageSize: 10);

        _productRepository.Setup(r => r.FindAsync(It.IsAny<ProductSearchSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        _productRepository.Setup(r => r.CountAsync(It.IsAny<ProductSearchSpecification>(), It.IsAny<CancellationToken>()))
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
    public async Task Handle_WithSearchTerm_ShouldFilterProducts()
    {
        // Arrange
        var query = new GetProductsQuery(SearchTerm: "laptop");

        _productRepository.Setup(r => r.FindAsync(It.IsAny<ProductSearchSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

        _productRepository.Setup(r => r.CountAsync(It.IsAny<ProductSearchSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _productRepository.Verify(r => r.FindAsync(
            It.IsAny<ProductSearchSpecification>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}