using Application.DTOs.Product;
using Shared.Models;

namespace Application.IServices
{
    public interface IProductService
    {
        Task<ProductDTO> CreateProductAsync(ProductDTO productDTO);
        Task DeleteProductAsync(Guid productId);
        Task<PaginatedResult<ProductListDTO>> GetAllProductsAsync(PaginatedFilterParams filterParams);
        Task<ProductDetailDTO?> GetProductByIdAsync(Guid productId);
        Task UpdateProductAsync(Guid productId, ProductUpdateDTO productUpdateDTO);
    }
}
