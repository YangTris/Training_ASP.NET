using System;

namespace Application.DTOs.Product
{
    public class ProductListDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? MainImageUrl { get; set; }
    }
}