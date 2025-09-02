using OrderManagement.Domain.Common;
using OrderManagement.Domain.Enums;
using OrderManagement.Domain.Events;
using OrderManagement.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Domain.Entities
{
    public class Order : BaseEntity
    {
        public string OrderNumber { get; private set; } = string.Empty;
        public Guid CustomerId { get; private set; }
        public Customer Customer { get; private set; } = null!;
        public OrderStatus Status { get; private set; }
        public DateTime OrderDate { get; private set; }
        public DateTime? ShippedDate { get; private set; }
        public DateTime? DeliveredDate { get; private set; }
        public string? ShippingAddress { get; private set; }
        public string? Notes { get; private set; }

        private readonly List<OrderItem> _orderItems = new();
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

        public Money TotalAmount => _orderItems.Aggregate(
            new Money(0),
            (sum, item) => sum.Add(item.TotalPrice)
        );

        public int TotalItems => _orderItems.Sum(item => item.Quantity);

        private Order() { } // EF Core

        public Order(Customer customer, string? shippingAddress = null, string? notes = null)
        {
            Customer = customer ?? throw new ArgumentNullException(nameof(customer));
            CustomerId = customer.Id;
            OrderNumber = GenerateOrderNumber();
            Status = OrderStatus.Pending;
            OrderDate = DateTime.UtcNow;
            ShippingAddress = shippingAddress;
            Notes = notes;

            // Raise domain event
            AddDomainEvent(new OrderCreatedEvent(Id, CustomerId));
        }

        public void AddOrderItem(Product product, int quantity)
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException("Cannot modify confirmed order");

            var existingItem = _orderItems.FirstOrDefault(x => x.ProductId == product.Id);

            if (existingItem != null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                var orderItem = new OrderItem(product, quantity);
                _orderItems.Add(orderItem);
            }

            SetUpdatedAt();
        }

        public void RemoveOrderItem(Guid productId)
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException("Cannot modify confirmed order");

            var item = _orderItems.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                _orderItems.Remove(item);
                SetUpdatedAt();
            }
        }

        public void UpdateOrderItemQuantity(Guid productId, int newQuantity)
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException("Cannot modify confirmed order");

            var item = _orderItems.FirstOrDefault(x => x.ProductId == productId);
            if (item == null)
                throw new ArgumentException("Order item not found", nameof(productId));

            if (newQuantity <= 0)
            {
                RemoveOrderItem(productId);
            }
            else
            {
                item.UpdateQuantity(newQuantity);
                SetUpdatedAt();
            }
        }

        public void Confirm()
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException($"Cannot confirm order with status {Status}");

            if (!_orderItems.Any())
                throw new InvalidOperationException("Cannot confirm empty order");

            var previousStatus = Status;
            Status = OrderStatus.Confirmed;
            SetUpdatedAt();

            AddDomainEvent(new OrderStatusChangedEvent(Id, previousStatus, Status));
        }

        public void StartProcessing()
        {
            if (Status != OrderStatus.Confirmed)
                throw new InvalidOperationException($"Cannot process order with status {Status}");

            var previousStatus = Status;
            Status = OrderStatus.Processing;
            SetUpdatedAt();

            AddDomainEvent(new OrderStatusChangedEvent(Id, previousStatus, Status));
        }

        public void Ship(DateTime? shippedDate = null)
        {
            if (Status != OrderStatus.Processing)
                throw new InvalidOperationException($"Cannot ship order with status {Status}");

            var previousStatus = Status;
            Status = OrderStatus.Shipped;
            ShippedDate = shippedDate ?? DateTime.UtcNow;
            SetUpdatedAt();

            AddDomainEvent(new OrderStatusChangedEvent(Id, previousStatus, Status));
        }

        public void Deliver(DateTime? deliveredDate = null)
        {
            if (Status != OrderStatus.Shipped)
                throw new InvalidOperationException($"Cannot deliver order with status {Status}");

            var previousStatus = Status;
            Status = OrderStatus.Delivered;
            DeliveredDate = deliveredDate ?? DateTime.UtcNow;
            SetUpdatedAt();

            AddDomainEvent(new OrderStatusChangedEvent(Id, previousStatus, Status));
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Delivered)
                throw new InvalidOperationException("Cannot cancel delivered order");

            if (Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Order is already cancelled");

            var previousStatus = Status;
            Status = OrderStatus.Cancelled;
            SetUpdatedAt();

            AddDomainEvent(new OrderStatusChangedEvent(Id, previousStatus, Status));
        }

        public void UpdateShippingAddress(string shippingAddress)
        {
            if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
                throw new InvalidOperationException("Cannot update shipping address for shipped/delivered order");

            ShippingAddress = shippingAddress;
            SetUpdatedAt();
        }

        public void UpdateNotes(string? notes)
        {
            Notes = notes;
            SetUpdatedAt();
        }

        public bool CanBeModified() => Status == OrderStatus.Pending;

        public bool CanBeCancelled() => Status != OrderStatus.Delivered && Status != OrderStatus.Cancelled;

        private static string GenerateOrderNumber()
        {
            // Generate order number: ORD-YYYYMMDD-XXXX
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = new Random().Next(1000, 9999);
            return $"ORD-{datePart}-{randomPart}";
        }

        public void ValidateForConfirmation()
        {
            if (!_orderItems.Any())
                throw new InvalidOperationException("Order must have at least one item");

            if (string.IsNullOrWhiteSpace(ShippingAddress))
                throw new InvalidOperationException("Shipping address is required for confirmation");

            // Check stock availability for all items
            foreach (var item in _orderItems)
            {
                if (!item.Product.IsInStock(item.Quantity))
                {
                    throw new InvalidOperationException($"Insufficient stock for product {item.Product.Name}");
                }
            }
        }

        // Helper method to get order summary
        public string GetOrderSummary()
        {
            var itemCount = TotalItems;
            var itemText = itemCount == 1 ? "item" : "items";
            return $"{OrderNumber}: {itemCount} {itemText}, Total: {TotalAmount}, Status: {Status}";
        }
    }
}
