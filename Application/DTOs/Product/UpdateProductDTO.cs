using System;

namespace Application.DTOs.Product
{
    public class UpdateProductDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }
}
