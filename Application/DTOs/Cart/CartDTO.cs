using System;

namespace Application.DTOs.Cart
{
    public class CartDTO
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ICollection<CartItemDTO> Items { get; set; } = new List<CartItemDTO>();
        public decimal TotalAmount => Items.Sum(item => item.TotalPrice);
    }
}
