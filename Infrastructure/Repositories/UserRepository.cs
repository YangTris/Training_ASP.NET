using Core.Entities;
using Core.Exceptions;
using Core.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserRepository(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<User> CreateAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return user;
        }

        public async Task DeleteAsync(User user)
        {
            await _userManager.DeleteAsync(user);
        }

        public async Task<User?> GetByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<User?> GetWithOrderByIdAsync(string userId)
        {
            return await _userManager.Users
                 .Include(u => u.Orders)
                 .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<PaginatedResult<User>> GetUsersAsync(PaginatedFilterParams filterParams)
        {
            IQueryable<User> query = _userManager.Users;

            if (!string.IsNullOrWhiteSpace(filterParams.SearchTerm))
            {
                var searchTerm = filterParams.SearchTerm.ToLower();
                query = query.Where(u => u.FullName.Contains(filterParams.SearchTerm));
            }

            query = filterParams.SortBy.ToLower() switch
            {
                "fullname" => filterParams.IsDescending
                    ? query.OrderByDescending(u => u.FullName)
                    : query.OrderBy(u => u.FullName),

                "email" => filterParams.IsDescending
                    ? query.OrderByDescending(u => u.Email)
                    : query.OrderBy(u => u.Email),

                _ => filterParams.IsDescending
                    ? query.OrderByDescending(u => u.CreatedAt)
                    : query.OrderBy(u => u.CreatedAt),
            };

            var totalItem = await query.CountAsync();
            var items = await query
                .Skip((filterParams.PageNumber - 1) * filterParams.PageSize)
                .Take(filterParams.PageSize)
                .ToListAsync();

            return new PaginatedResult<User>
            {
                Items = items,
                TotalItems = totalItem,
                PageNumber = filterParams.PageNumber,
                PageSize = filterParams.PageSize
            };
        }

        public async Task UpdateAsync(User user)
        {
            await _userManager.UpdateAsync(user);
        }

        public async Task AssignRoleAsync(User user, string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                throw new NotFoundException($"Role '{roleName}' does not exist.");
            }
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}