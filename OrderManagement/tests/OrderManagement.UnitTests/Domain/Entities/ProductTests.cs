using OrderManagement.Domain.Entities;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.UnitTests.Domain.Entities
{
    public class ProductTests
    {
        [Fact]
        public void CreateProduct_WithValidData_ShouldCreateCorrectly()
        {
            // Arrange
            var name = "Test Product";
            var price = new Money(99.99m, "USD");
            var stockQuantity = 50;
            var description = "A test product";
            var category = "Test Category";

            // Act
            var product = new Product(name, price, stockQuantity, description, category);

            // Assert
            Assert.Equal(name, product.Name);
            Assert.Equal(price, product.Price);
            Assert.Equal(stockQuantity, product.StockQuantity);
            Assert.Equal(description, product.Description);
            Assert.Equal(category, product.Category);
            Assert.True(product.IsActive);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void CreateProduct_WithInvalidName_ShouldThrowException(string name)
        {
            // Arrange
            var price = new Money(99.99m, "USD");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Product(name, price, 10));
        }

        [Fact]
        public void CreateProduct_WithNullPrice_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Product("Test Product", null, 10));
        }

        [Fact]
        public void CreateProduct_WithNegativeStock_ShouldThrowException()
        {
            // Arrange
            var price = new Money(99.99m, "USD");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Product("Test Product", price, -1));
        }

        [Fact]
        public void UpdateStock_WithValidQuantity_ShouldUpdateCorrectly()
        {
            // Arrange
            var product = new Product("Test Product", new Money(99.99m, "USD"), 10);

            // Act
            product.UpdateStock(25);

            // Assert
            Assert.Equal(25, product.StockQuantity);
            Assert.NotNull(product.UpdatedAt);
        }

        [Fact]
        public void UpdateStock_WithNegativeQuantity_ShouldThrowException()
        {
            // Arrange
            var product = new Product("Test Product", new Money(99.99m, "USD"), 10);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => product.UpdateStock(-1));
        }

        [Theory]
        [InlineData(10, 5, true)]
        [InlineData(10, 10, true)]
        [InlineData(10, 15, false)]
        [InlineData(0, 1, false)]
        public void IsInStock_WithVariousQuantities_ShouldReturnCorrectResult(int stockQuantity, int requestedQuantity, bool expectedResult)
        {
            // Arrange
            var product = new Product("Test Product", new Money(99.99m, "USD"), stockQuantity);

            // Act
            var result = product.IsInStock(requestedQuantity);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void Deactivate_ShouldSetIsActiveFalse()
        {
            // Arrange
            var product = new Product("Test Product", new Money(99.99m, "USD"), 10);

            // Act
            product.Deactivate();

            // Assert
            Assert.False(product.IsActive);
            Assert.NotNull(product.UpdatedAt);
        }

        [Fact]
        public void Activate_ShouldSetIsActiveTrue()
        {
            // Arrange
            var product = new Product("Test Product", new Money(99.99m, "USD"), 10);
            product.Deactivate();

            // Act
            product.Activate();

            // Assert
            Assert.True(product.IsActive);
            Assert.NotNull(product.UpdatedAt);
        }

        [Fact]
        public void IsInStock_WhenProductInactive_ShouldReturnFalse()
        {
            // Arrange
            var product = new Product("Test Product", new Money(99.99m, "USD"), 10);
            product.Deactivate();

            // Act
            var result = product.IsInStock(5);

            // Assert
            Assert.False(result);
        }
    }

}
