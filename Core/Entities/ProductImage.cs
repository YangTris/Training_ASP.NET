namespace Core.Entities
{
    public class ProductImage
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }

        // Navigation
        public Product? Product { get; set; }
    }
}
