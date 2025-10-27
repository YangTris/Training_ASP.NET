namespace Application.DTOs.User
{
    public class UserUpdateDTO
    {
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }
}