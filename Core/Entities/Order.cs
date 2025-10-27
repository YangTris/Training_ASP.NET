using Core.Entities.Enums;

namespace Core.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string ShippingAddress { get; set; } = string.Empty;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CashOnDelivery;

        // Navigation
        public User? User { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}