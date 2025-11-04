using Application.DTOs.Category;
using Application.DTOs.Product;
using Application.IServices;
using Core.Entities;
using Core.Exceptions;
using Core.IRepositories;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<CategoryDetailDTO> CreateCategoryAsync(CreateCategoryDTO createCategoryDTO)
        {
            var category = new Category
            {
                Name = createCategoryDTO.Name,
                Description = createCategoryDTO.Description
            };

            var created = await _categoryRepository.CreateAsync(category);

            return new CategoryDetailDTO
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description,
                IsDeleted = created.IsDeleted,
                CreatedAt = created.CreatedAt,
                Products = new List<ProductListDTO>()
            };
        }

        public async Task DeleteCategoryAsync(Guid categoryId)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(categoryId);
            if (existingCategory == null)
            {
                throw new NotFoundException($"Category {categoryId} not found");
            }
            await _categoryRepository.DeleteAsync(existingCategory);
        }

        public async Task<IEnumerable<CategoryListDTO>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(c => new CategoryListDTO
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            });
        }

        public async Task<CategoryDetailDTO?> GetCategoryByIdAsync(Guid categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new NotFoundException($"Category {categoryId} not found");
            }
            return new CategoryDetailDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.CreatedAt,
                IsDeleted = category.IsDeleted,

                Products = category.Products?.Select(p => new ProductListDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price
                }).ToList() ?? new List<ProductListDTO>()
            };
        }

        public async Task UpdateCategoryAsync(Guid categoryId, UpdateCategoryDTO updateCategoryDTO)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(categoryId);
            if (existingCategory == null)
            {
                throw new NotFoundException($"Category {categoryId} not found");
            }
            existingCategory.Name = updateCategoryDTO.Name;
            existingCategory.Description = updateCategoryDTO.Description;

            await _categoryRepository.UpdateAsync(existingCategory);
        }
    }
}