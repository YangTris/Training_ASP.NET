using Core.Entities;
using Shared.Models;

namespace Core.IRepositories
{
    public interface IProductRepository
    {
        Task<PaginatedResult<Product>> GetProductsAsync(PaginatedFilterParams filterParams);
        Task<Product?> GetByIdAsync(string productId);
        Task<Product> CreateAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
    }
}