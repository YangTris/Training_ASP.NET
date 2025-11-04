using Application.DTOs.Category;

namespace Application.IServices
{
    public interface ICategoryService
    {
        Task<CategoryDetailDTO> CreateCategoryAsync(CreateCategoryDTO createCategoryDTO);
        Task DeleteCategoryAsync(Guid categoryId);
        Task<IEnumerable<CategoryListDTO>> GetAllCategoriesAsync();
        Task<CategoryDetailDTO?> GetCategoryByIdAsync(Guid categoryId);
        Task UpdateCategoryAsync(Guid categoryId, UpdateCategoryDTO updateCategoryDTO);
    }
}