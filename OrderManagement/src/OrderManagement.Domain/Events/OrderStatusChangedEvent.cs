using OrderManagement.Domain.Common;
using OrderManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Domain.Events
{
    public class OrderStatusChangedEvent : IDomainEvent
    {
        public Guid OrderId { get; }
        public OrderStatus PreviousStatus { get; }
        public OrderStatus NewStatus { get; }
        public DateTime OccurredOn { get; }

        public OrderStatusChangedEvent(Guid orderId, OrderStatus previousStatus, OrderStatus newStatus)
        {
            OrderId = orderId;
            PreviousStatus = previousStatus;
            NewStatus = newStatus;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
