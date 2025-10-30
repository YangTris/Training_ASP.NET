using Application.DTOs.Product;
using Shared.Models;

namespace Application.IServices
{
    public interface IProductService
    {
        Task<ProductDetailDTO> CreateProductAsync(CreateProductDTO createProductDTO);
        Task DeleteProductAsync(Guid productId);
        Task<PaginatedResult<ProductListDTO>> GetAllProductsAsync(PaginatedFilterParams filterParams);
        Task<ProductDetailDTO?> GetProductByIdAsync(Guid productId);
        Task UpdateProductAsync(Guid productId, UpdateProductDTO updateProductDTO);
    }
}
