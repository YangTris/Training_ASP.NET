using Application.DTOs.Order;
using Application.DTOs.User;
using Application.IServices;
using Core.Entities;
using Core.Exceptions;
using Core.IRepositories;
using Microsoft.AspNetCore.Identity;
using Shared.Models;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        public UserService(IUserRepository userRepository, UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public async Task AssignRoleAsync(string userId, string role = "User")
        {
            var existingUser = await _userRepository.GetByIdAsync(userId);
            if (existingUser == null)
            {
                throw new NotFoundException($"User {userId} not found");
            }

            await _userRepository.AssignRoleAsync(existingUser, role);
        }

        public async Task<UserCreateResponseDTO> CreateUserAsync(UserCreateRequestDTO userCreateRequestDTO)
        {
            var emailValidator = new System.ComponentModel.DataAnnotations.EmailAddressAttribute();
            if (string.IsNullOrWhiteSpace(userCreateRequestDTO.Email) || !emailValidator.IsValid(userCreateRequestDTO.Email))
                throw new BadRequestException("Email is not a valid email address.");

            var existingByEmail = await _userManager.FindByEmailAsync(userCreateRequestDTO.Email);
            if (existingByEmail != null)
                throw new ConflictException($"Email {userCreateRequestDTO.Email} is already in use.");

            var user = new User
            {
                FullName = userCreateRequestDTO.FullName,
                Email = userCreateRequestDTO.Email,
                UserName = userCreateRequestDTO.Email
            };

            var createdUser = await _userRepository.CreateAsync(user, userCreateRequestDTO.Password);

            await _userRepository.AssignRoleAsync(createdUser, "User");

            return new UserCreateResponseDTO
            {
                Id = createdUser.Id,
                FullName = createdUser.FullName,
                Email = createdUser.Email ?? string.Empty,
                CreatedAt = createdUser.CreatedAt
            };
        }

        public async Task DeleteUserAsync(string id)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null) throw new NotFoundException($"User {id} not found");

            else
                await _userRepository.DeleteAsync(existingUser);
        }

        public async Task<PaginatedResult<UserListDTO>> GetUser(PaginatedFilterParams filterParams)
        {
            var paginatedUsers = await _userRepository.GetUsersAsync(filterParams);

            var userListDTOs = paginatedUsers.Items.Select(u => new UserListDTO
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email ?? string.Empty,
                CreatedAt = u.CreatedAt,
            }).ToList();

            return new PaginatedResult<UserListDTO>
            {
                Items = userListDTOs,
                TotalItems = paginatedUsers.TotalItems,
                PageNumber = paginatedUsers.PageNumber,
                PageSize = paginatedUsers.PageSize
            };
        }

        public async Task<UserDetailDTO?> GetUserByIdAsync(string id)
        {
            var user = await _userRepository.GetWithOrderByIdAsync(id);
            if (user == null) throw new NotFoundException($"User {id} not found");

            var userDetailDTO = new UserDetailDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                CreatedAt = user.CreatedAt.DateTime,
                OrderDetailDTOs = user.Orders?.Select(o => new OrderDetailDTO
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    TotalAmount = o.TotalAmount,
                    OrderDate = o.OrderDate.DateTime
                }).ToList() ?? new List<OrderDetailDTO>()
            };

            return userDetailDTO;
        }

        public async Task UpdateUserAsync(string userId, UserUpdateDTO user)
        {
            var existingUser = await _userRepository.GetByIdAsync(userId);
            if (existingUser == null)
            {
                throw new NotFoundException($"User {userId} not found");
            }

            existingUser.SetUser(user.FullName);
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;

            await _userRepository.UpdateAsync(existingUser);
        }
    }
}