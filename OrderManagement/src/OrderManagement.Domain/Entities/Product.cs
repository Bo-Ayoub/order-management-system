using OrderManagement.Domain.Common;
using OrderManagement.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public Money Price { get; private set; } = null!;
        public int StockQuantity { get; private set; }
        public string? Category { get; private set; }
        public bool IsActive { get; private set; } = true;

        private Product() { } // EF Core

        public Product(string name, Money price, int stockQuantity, string? description = null, string? category = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty", nameof(name));

            Name = name;
            Description = description;
            Price = price ?? throw new ArgumentNullException(nameof(price));
            StockQuantity = stockQuantity >= 0 ? stockQuantity : throw new ArgumentException("Stock quantity cannot be negative");
            Category = category;
        }

        public void UpdateStock(int newQuantity)
        {
            if (newQuantity < 0)
                throw new ArgumentException("Stock quantity cannot be negative", nameof(newQuantity));

            StockQuantity = newQuantity;
            SetUpdatedAt();
        }

        public void UpdatePrice(Money newPrice)
        {
            Price = newPrice ?? throw new ArgumentNullException(nameof(newPrice));
            SetUpdatedAt();
        }

        public bool IsInStock(int requestedQuantity) => IsActive && StockQuantity >= requestedQuantity;

        public void Deactivate()
        {
            IsActive = false;
            SetUpdatedAt();
        }

        public void Activate()
        {
            IsActive = true;
            SetUpdatedAt();
        }
    }
}
