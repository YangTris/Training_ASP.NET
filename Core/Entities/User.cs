using Core.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public Cart? Cart { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();

        public User() { }

        public User(string fullName)
        {
            SetUser(fullName);
        }

        public void SetUser(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new DomainException("Full name cannot be empty.");

            FullName = fullName;
            UserName = fullName;
        }
    }
}