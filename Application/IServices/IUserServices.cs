using Application.DTOs.User;
using Shared.Models;

namespace Application.IServices
{
    public interface IUserService
    {
        Task<PaginatedResult<UserListDTO>> GetUser(PaginatedFilterParams filterParams);
        Task<UserDetailDTO?> GetUserByIdAsync(string id);
        Task<UserCreateResponseDTO> CreateUserAsync(UserCreateRequestDTO userCreateRequestDTO);
        Task UpdateUserAsync(string userId, UserUpdateDTO user);
        Task DeleteUserAsync(string id);
        Task AssignRoleAsync(string userId, string role);
    }
}