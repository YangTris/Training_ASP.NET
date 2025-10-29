using Core.Entities.Enums;
using System;

namespace Application.DTOs.Order;

public class OrderDetailDTO
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTimeOffset OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public PaymentMethod PaymentMethod { get; set; }
    public ICollection<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
}

