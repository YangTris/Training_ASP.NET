using System;

namespace Application.DTOs.Product
{
    public class ProductDetailDTO : ProductListDTO
    {
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public Guid CategoryId { get; set; }
    }
}
