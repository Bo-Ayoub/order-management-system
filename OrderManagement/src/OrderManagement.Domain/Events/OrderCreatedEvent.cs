using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Events
{
    public class OrderCreatedEvent : IDomainEvent
    {
        public Guid OrderId { get; }
        public Guid CustomerId { get; }
        public DateTime OccurredOn { get; }

        public OrderCreatedEvent(Guid orderId, Guid customerId)
        {
            OrderId = orderId;
            CustomerId = customerId;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
