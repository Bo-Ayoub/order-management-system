using OrderManagement.Domain.Common;
using OrderManagement.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public Guid ProductId { get; private set; }
        public Product Product { get; private set; } = null!;
        public int Quantity { get; private set; }
        public Money UnitPrice { get; private set; } = null!;

        public Money TotalPrice => UnitPrice.Multiply(Quantity);

        private OrderItem() { } // EF Core

        public OrderItem(Product product, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));

            Product = product ?? throw new ArgumentNullException(nameof(product));
            ProductId = product.Id;
            Quantity = quantity;
            UnitPrice = product.Price;

            if (!product.IsInStock(quantity))
                throw new InvalidOperationException($"Insufficient stock for product {product.Name}");
        }

        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(newQuantity));

            if (!Product.IsInStock(newQuantity))
                throw new InvalidOperationException($"Insufficient stock for product {Product.Name}");

            Quantity = newQuantity;
            SetUpdatedAt();
        }
    }
}
