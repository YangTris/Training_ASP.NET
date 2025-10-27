namespace Core.Entities
{
    public class Cart
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;

        // Navigation
        public User? User { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }
    }
}
