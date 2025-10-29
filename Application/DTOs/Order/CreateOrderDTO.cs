using Core.Entities.Enums;
using System;

namespace Application.DTOs.Order
{
    public class CreateOrderDTO
    {
        public string ShippingAddress { get; set; } = string.Empty;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CashOnDelivery;
    }
}
