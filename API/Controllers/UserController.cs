using Application.DTOs;
using Application.DTOs.User;
using Application.IServices;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserListDTO>>> GetUsers([FromQuery] PaginatedFilterParams filterParams)
        {
            var users = await _userService.GetUser(filterParams);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDetailDTO>> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole(string id, [FromQuery] string role)
        {
            await _userService.AssignRoleAsync(id, role);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateRequestDTO userCreateRequestDTO)
        {
            var createdUser = await _userService.CreateUserAsync(userCreateRequestDTO);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateDTO userUpdateDto)
        {
            await _userService.UpdateUserAsync(id, userUpdateDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
    }
}