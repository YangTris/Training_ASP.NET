using System;

namespace Application.DTOs.Cart
{
    public class AddCartItemDTO
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
