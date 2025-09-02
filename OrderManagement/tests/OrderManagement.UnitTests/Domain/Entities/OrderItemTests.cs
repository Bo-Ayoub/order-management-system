using OrderManagement.Domain.Entities;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.UnitTests.Domain.Entities
{
    public class OrderItemTests
    {
        private readonly Product _testProduct;

        public OrderItemTests()
        {
            _testProduct = new Product("Test Product", new Money(25.50m, "USD"), 100);
        }

        [Fact]
        public void CreateOrderItem_WithValidData_ShouldCreateCorrectly()
        {
            // Act
            var orderItem = new OrderItem(_testProduct, 3);

            // Assert
            Assert.Equal(_testProduct.Id, orderItem.ProductId);
            Assert.Equal(_testProduct, orderItem.Product);
            Assert.Equal(3, orderItem.Quantity);
            Assert.Equal(_testProduct.Price, orderItem.UnitPrice);
            Assert.Equal(new Money(76.50m, "USD"), orderItem.TotalPrice);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void CreateOrderItem_WithInvalidQuantity_ShouldThrowException(int quantity)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new OrderItem(_testProduct, quantity));
        }

        [Fact]
        public void CreateOrderItem_WithNullProduct_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new OrderItem(null, 1));
        }

        [Fact]
        public void CreateOrderItem_WhenProductOutOfStock_ShouldThrowException()
        {
            // Arrange
            var product = new Product("Limited Product", new Money(10m, "USD"), 5);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new OrderItem(product, 10));
        }

        [Fact]
        public void UpdateQuantity_WithValidQuantity_ShouldUpdateCorrectly()
        {
            // Arrange
            var orderItem = new OrderItem(_testProduct, 2);

            // Act
            orderItem.UpdateQuantity(5);

            // Assert
            Assert.Equal(5, orderItem.Quantity);
            Assert.NotNull(orderItem.UpdatedAt);
        }

        [Fact]
        public void UpdateQuantity_WithInsufficientStock_ShouldThrowException()
        {
            // Arrange
            var limitedProduct = new Product("Limited Product", new Money(10m, "USD"), 3);
            var orderItem = new OrderItem(limitedProduct, 2);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => orderItem.UpdateQuantity(5));
        }

        [Fact]
        public void TotalPrice_ShouldCalculateCorrectly()
        {
            // Arrange
            var product = new Product("Expensive Item", new Money(199.99m, "USD"), 10);
            var orderItem = new OrderItem(product, 3);

            // Act
            var totalPrice = orderItem.TotalPrice;

            // Assert
            Assert.Equal(new Money(599.97m, "USD"), totalPrice);
        }
    }
}
