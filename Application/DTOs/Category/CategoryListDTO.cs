namespace Application.DTOs.Category
{
    public class CategoryListDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}