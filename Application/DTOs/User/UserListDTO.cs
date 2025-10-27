namespace Application.DTOs.User
{
    public class UserListDTO
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
    }
}