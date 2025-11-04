using Core.Entities.Enums;
using System;

namespace Application.DTOs.Order;

public class OrderDetailDTO : OrderListDTO
{
    public string ShippingAddress { get; set; } = string.Empty;
    public ICollection<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
}

