using Application.DTOs.Category;

namespace Application.IServices
{
    public interface ICategoryService
    {
        Task<CategoryDTO> CreateCategoryAsync(CategoryDTO categoryDTO);
        Task DeleteCategoryAsync(Guid categoryId);
        Task<IEnumerable<CategoryListDTO>> GetAllCategoriesAsync();
        Task<CategoryDetailDTO?> GetCategoryByIdAsync(Guid categoryId);
        Task UpdateCategoryAsync(Guid categoryId, CategoryUpdateDTO categoryUpdateDTO);
    }
}