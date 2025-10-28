using Application.DTOs.Cart;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID not found in token");
        }

        [HttpGet]
        public async Task<ActionResult<CartDTO>> GetCart()
        {
            var userId = GetUserId();
            var cart = await _cartService.GetUserCartAsync(userId);
            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<ActionResult<CartDTO>> AddItem([FromBody] AddCartItemDTO itemDTO)
        {
            var userId = GetUserId();
            var cart = await _cartService.AddItemToCartAsync(userId, itemDTO);
            return Ok(cart);
        }

        [HttpPut("items/{cartItemId}")]
        public async Task<ActionResult<CartDTO>> UpdateItem(Guid cartItemId, [FromBody] UpdateCartItemDTO updateDTO)
        {
            var userId = GetUserId();
            var cart = await _cartService.UpdateCartItemAsync(userId, cartItemId, updateDTO);
            return Ok(cart);
        }

        [HttpDelete("items/{cartItemId}")]
        public async Task<IActionResult> RemoveItem(Guid cartItemId)
        {
            var userId = GetUserId();
            await _cartService.RemoveCartItemAsync(userId, cartItemId);
            return NoContent();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            await _cartService.ClearCartAsync(userId);
            return NoContent();
        }
    }
}
