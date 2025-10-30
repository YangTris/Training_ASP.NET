using System;

namespace Application.DTOs.Product
{
    public class ProductDetailDTO : ProductListDTO
    {
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public Guid CategoryId { get; set; }
        public IEnumerable<ProductImageDTO> Images { get; set; } = new List<ProductImageDTO>();
    }
}
