namespace Shared.Models
{
    public class PaginatedFilterParams
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; } = "CreatedAt";
        public bool IsDescending { get; set; } = false;
    }
}