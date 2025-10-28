using Application.DTOs.Product;
using Application.IServices;
using Core.Entities;
using Core.Exceptions;
using Core.IRepositories;
using Shared.Models;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<ProductDTO> CreateProductAsync(ProductDTO productDTO)
        {
            var categoryExists = await _categoryRepository.GetByIdAsync(productDTO.CategoryId);
            if (categoryExists == null)
                throw new NotFoundException($"Category {productDTO.CategoryId} does not exist");
            if (productDTO.Price < 0)
                throw new BadRequestException("Price cannot be negative");

            var product = new Product
            {
                Name = productDTO.Name,
                Description = productDTO.Description,
                Price = productDTO.Price,
                CategoryId = productDTO.CategoryId,
                Images = new List<ProductImage>
                {
                    new ProductImage
                    {
                        ImageUrl = "https://vyghvmdysxqvocgvytoe.supabase.co/storage/v1/object/public/Training_img/default_img.jpg",
                        IsMain = true
                    }
                }
            };

            var created = await _productRepository.CreateAsync(product);

            return new ProductDTO
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description,
                Price = created.Price,
                CategoryId = created.CategoryId
            };
        }

        public async Task DeleteProductAsync(Guid productId)
        {
            var existing = await _productRepository.GetByIdAsync(productId);
            if (existing == null)
                throw new NotFoundException($"Product {productId} not found");

            await _productRepository.DeleteAsync(existing);
        }

        public async Task<PaginatedResult<ProductListDTO>> GetAllProductsAsync(PaginatedFilterParams filterParams)
        {
            var paginatedProducts = await _productRepository.GetProductsAsync(filterParams);

            var dtoItems = paginatedProducts.Items.Select(p => new ProductListDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price
            });

            return new PaginatedResult<ProductListDTO>
            {
                Items = dtoItems,
                TotalItems = paginatedProducts.TotalItems,
                PageNumber = paginatedProducts.PageNumber,
                PageSize = paginatedProducts.PageSize
            };
        }

        public async Task<ProductDetailDTO?> GetProductByIdAsync(Guid productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                throw new NotFoundException($"Product {productId} not found");

            return new ProductDetailDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                CategoryId = product.CategoryId
            };
        }

        public async Task UpdateProductAsync(Guid productId, ProductUpdateDTO productUpdateDTO)
        {
            var existing = await _productRepository.GetByIdAsync(productId);
            if (existing == null)
                throw new NotFoundException($"Product {productId} not found");

            existing.Name = productUpdateDTO.Name;
            existing.Description = productUpdateDTO.Description;
            existing.Price = productUpdateDTO.Price;
            existing.UpdatedAt = DateTimeOffset.UtcNow;

            await _productRepository.UpdateAsync(existing);
        }
    }
}
