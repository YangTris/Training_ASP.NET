using Core.Entities.Enums;
using System;

namespace Application.DTOs.Order
{
    public class UpdateOrderStatusDTO
    {
        public OrderStatus Status { get; set; }
    }
}
