using Core.Entities;
using Shared.Models;

namespace Core.IRepositories
{
    public interface IUserRepository
    {
        Task<PaginatedResult<User>> GetUsersAsync(PaginatedFilterParams filterParams);
        Task<User?> GetByIdAsync(string userId);
        Task<User?> GetWithOrderByIdAsync(string userId);
        Task<User> CreateAsync(User user, string password);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task AssignRoleAsync(User user, string roleName);
    }
}