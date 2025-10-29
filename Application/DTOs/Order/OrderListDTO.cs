using Core.Entities.Enums;
using System;

namespace Application.DTOs.Order
{
    public class OrderListDTO
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTimeOffset OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public int ItemCount { get; set; }
    }
}
