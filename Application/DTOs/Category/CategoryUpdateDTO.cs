using System;

namespace Application.DTOs.Category
{
    public class CategoryUpdateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}