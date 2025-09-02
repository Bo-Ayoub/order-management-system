using OrderManagement.Application.Features.Orders.Specifications;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.Domain.ValueObjects;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace OrderManagement.UnitTests.Application.Features.Orders.Specifications
{
    public class OrderSpecificationTests
    {
        private readonly Customer _customer;
        private readonly List<Product> _products;
        private readonly List<Order> _orders;

        public OrderSpecificationTests()
        {
            // Arrange customer
            _customer = new Customer("Test", "Customer", new Email("test@example.com"));

            // Arrange some dummy products
            _products = new List<Product>
            {
                new Product("Product 1", new Money(10, "USD"), 100),
                new Product("Product 2", new Money(20, "USD"), 50),
                new Product("Product 3", new Money(30, "USD"), 30)
            };

            // Create orders
            var order1 = new Order(_customer, "Address 1");
            order1.AddOrderItem(_products[0], 2); // ensure not empty

            var order2 = new Order(_customer, "Address 2");
            order2.AddOrderItem(_products[1], 1);
            order2.Confirm();

            var order3 = new Order(_customer, "Address 3");
            order3.AddOrderItem(_products[2], 3);
            order3.Confirm();
            order3.StartProcessing();

            _orders = new List<Order> { order1, order2, order3 };
        }

        [Fact]
        public void OrdersByCustomerSpecification_ShouldFilterByCustomer()
        {
            // Arrange
            var specification = new OrdersByCustomerSpecification(_customer.Id);

            // Act
            var result = _orders.Where(specification.Criteria.Compile()).ToList();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.All(result, order => Assert.Equal(_customer.Id, order.CustomerId));
        }

        [Fact]
        public void OrdersByStatusSpecification_ShouldFilterByStatus()
        {
            // Arrange
            var specification = new OrdersByStatusSpecification(OrderStatus.Confirmed);

            // Act
            var result = _orders.Where(specification.Criteria.Compile()).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal(OrderStatus.Confirmed, result.First().Status);
        }

        [Fact]
        public void PendingOrdersSpecification_ShouldReturnOnlyPendingOrders()
        {
            // Arrange
            var specification = new PendingOrdersSpecification();

            // Act
            var result = _orders.Where(specification.Criteria.Compile()).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal(OrderStatus.Pending, result.First().Status);
        }
    }
}
