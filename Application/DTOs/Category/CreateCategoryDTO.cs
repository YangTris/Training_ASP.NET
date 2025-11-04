using System;

namespace Application.DTOs.Category
{
    public class CreateCategoryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}