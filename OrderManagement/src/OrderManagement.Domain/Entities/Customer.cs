using OrderManagement.Domain.Common;
using OrderManagement.Domain.ValueObjects;


namespace OrderManagement.Domain.Entities
{
    public  class Customer : BaseEntity
    {
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public Email Email { get; private set; } = null!;
        public string? PhoneNumber { get; private set; }

        private readonly List<Order> _orders = new();
        public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

        private Customer() { } // EF Core

        public Customer(string firstName, string lastName, Email email, string? phoneNumber = null)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty", nameof(firstName));

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty", nameof(lastName));

            FirstName = firstName;
            LastName = lastName;
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PhoneNumber = phoneNumber;
        }

        public string FullName => $"{FirstName} {LastName}";

        public void UpdateContactInfo(string firstName, string lastName, Email email, string? phoneNumber = null)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PhoneNumber = phoneNumber;

            SetUpdatedAt();
        }
    }
}
