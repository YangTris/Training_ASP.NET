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

        public async Task<ProductDetailDTO> CreateProductAsync(CreateProductDTO createProductDTO)
        {
            var categoryExists = await _categoryRepository.GetByIdAsync(createProductDTO.CategoryId);
            if (categoryExists == null)
                throw new NotFoundException($"Category {createProductDTO.CategoryId} does not exist");

            if (createProductDTO == null)
                throw new BadRequestException("Product data is required");
            if (string.IsNullOrWhiteSpace(createProductDTO.Name))
                throw new BadRequestException("Product name is required");
            if (createProductDTO.Price < 0)
                throw new BadRequestException("Price cannot be negative");

            var product = new Product
            {
                Name = createProductDTO.Name,
                Description = createProductDTO.Description,
                Price = createProductDTO.Price,
                CategoryId = createProductDTO.CategoryId,
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

            return new ProductDetailDTO
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description,
                Price = created.Price,
                CategoryId = created.CategoryId,
                CreatedAt = created.CreatedAt,
                UpdatedAt = created.UpdatedAt,
                MainImageUrl = created.Images?.FirstOrDefault(img => img.IsMain)?.ImageUrl,
                Images = created.Images?.Select(img => new ProductImageDTO
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl
                }) ?? Enumerable.Empty<ProductImageDTO>()
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
                Price = p.Price,
                MainImageUrl = p.Images?.FirstOrDefault(i => i.IsMain)?.ImageUrl
            }).ToList();

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
                CategoryId = product.CategoryId,
                MainImageUrl = product.Images?.FirstOrDefault(img => img.IsMain)?.ImageUrl,
                Images = product.Images?.Select(img => new ProductImageDTO
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl,
                }) ?? Enumerable.Empty<ProductImageDTO>()
            };
        }

        public async Task UpdateProductAsync(Guid productId, UpdateProductDTO updateProductDTO)
        {
            var existing = await _productRepository.GetByIdAsync(productId);
            if (existing == null)
                throw new NotFoundException($"Product {productId} not found");
            if (updateProductDTO.Price < 0)
                throw new BadRequestException("Price cannot be negative");

            existing.Name = updateProductDTO.Name;
            existing.Description = updateProductDTO.Description;
            existing.Price = updateProductDTO.Price;
            existing.UpdatedAt = DateTimeOffset.UtcNow;

            await _productRepository.UpdateAsync(existing);
        }
    }
}
