using Application.DTOs.User;
using Shared.Models;

namespace Application.IServices
{
    public interface IUserService
    {
        Task<PaginatedResult<UserListDTO>> GetUser(PaginatedFilterParams filterParams);
        Task<UserDetailDTO?> GetUserByIdAsync(string id);
        Task<UserDetailDTO> CreateUserAsync(CreateUserDTO createUserDTO);
        Task UpdateUserAsync(string userId, UpdateUserDTO updateUserDTO);
        Task DeleteUserAsync(string id);
        Task AssignRoleAsync(string userId, string role);
    }
}