using OrderManagement.Domain.Entities;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.UnitTests.Domain.Entities
{
    public class CustomerTests
    {
        [Fact]
        public void CreateCustomer_WithValidData_ShouldCreateCorrectly()
        {
            // Arrange
            var firstName = "John";
            var lastName = "Doe";
            var email = new Email("john@example.com");
            var phoneNumber = "+1-555-0123";

            // Act
            var customer = new Customer(firstName, lastName, email, phoneNumber);

            // Assert
            Assert.Equal(firstName, customer.FirstName);
            Assert.Equal(lastName, customer.LastName);
            Assert.Equal(email, customer.Email);
            Assert.Equal(phoneNumber, customer.PhoneNumber);
            Assert.Equal("John Doe", customer.FullName);
        }

        [Theory]
        [InlineData("", "Doe")]
        [InlineData(" ", "Doe")]
        [InlineData(null, "Doe")]
        public void CreateCustomer_WithInvalidFirstName_ShouldThrowException(string firstName, string lastName)
        {
            // Arrange
            var email = new Email("john@example.com");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Customer(firstName, lastName, email));
        }

        [Theory]
        [InlineData("John", "")]
        [InlineData("John", " ")]
        [InlineData("John", null)]
        public void CreateCustomer_WithInvalidLastName_ShouldThrowException(string firstName, string lastName)
        {
            // Arrange
            var email = new Email("john@example.com");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Customer(firstName, lastName, email));
        }

        [Fact]
        public void CreateCustomer_WithNullEmail_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Customer("John", "Doe", null));
        }

        [Fact]
        public void UpdateContactInfo_WithValidData_ShouldUpdateCorrectly()
        {
            // Arrange
            var customer = new Customer("John", "Doe", new Email("john@example.com"));
            var newEmail = new Email("john.doe@example.com");

            // Act
            customer.UpdateContactInfo("Jonathan", "Smith", newEmail, "+1-555-9876");

            // Assert
            Assert.Equal("Jonathan", customer.FirstName);
            Assert.Equal("Smith", customer.LastName);
            Assert.Equal(newEmail, customer.Email);
            Assert.Equal("+1-555-9876", customer.PhoneNumber);
            Assert.NotNull(customer.UpdatedAt);
        }
    }
}
