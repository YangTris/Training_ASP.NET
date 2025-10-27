using System;
using Application.DTOs.Product;

namespace Application.DTOs.Category
{
    public class CategoryDetailDTO : CategoryListDTO
    {
        public bool IsDeleted { get; set; } = false;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        // Navigation
        public ICollection<ProductListDTO>? Products { get; set; }
    }
}


