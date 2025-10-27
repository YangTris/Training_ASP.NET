using Application.DTOs.Category;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryListDTO>>> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{categoryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDetailDTO>> GetCategoryById(Guid categoryId)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDTO>> CreateCategory([FromBody] CategoryDTO categoryDTO)
        {
            var createdCategory = await _categoryService.CreateCategoryAsync(categoryDTO);
            return CreatedAtAction(nameof(GetCategoryById),
                new { categoryId = createdCategory.Id },
                createdCategory);
        }

        [HttpPut("{categoryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(Guid categoryId, [FromBody] CategoryUpdateDTO categoryUpdateDTO)
        {
            await _categoryService.UpdateCategoryAsync(categoryId, categoryUpdateDTO);
            return NoContent();
        }

        [HttpDelete("{categoryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(Guid categoryId)
        {
            await _categoryService.DeleteCategoryAsync(categoryId);
            return NoContent();
        }
    }
}