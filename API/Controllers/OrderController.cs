using Application.DTOs.Order;
using Application.IServices;
using Core.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID not found in token");
        }

        /// <summary>
        /// Create a new order from current user's cart
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<OrderDetailDTO>> CreateOrder([FromBody] CreateOrderDTO createOrderDTO)
        {
            var userId = GetUserId();
            var order = await _orderService.CreateOrderFromCartAsync(userId, createOrderDTO);
            return CreatedAtAction(nameof(GetOrderById), new { orderId = order.Id }, order);
        }

        /// <summary>
        /// Get all orders for the current user with pagination
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<OrderListDTO>>> GetMyOrders([FromQuery] PaginatedFilterParams filterParams)
        {
            var userId = GetUserId();
            var orders = await _orderService.GetUserOrdersAsync(userId, filterParams);
            return Ok(orders);
        }

        /// <summary>
        /// Get a specific order by ID (user can only access their own orders)
        /// </summary>
        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderDetailDTO>> GetOrderById(Guid orderId)
        {
            var userId = GetUserId();
            var order = await _orderService.GetOrderByIdAsync(userId, orderId);
            return Ok(order);
        }

        /// <summary>
        /// Update order status (Admin only)
        /// </summary>
        [HttpPatch("{orderId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(Guid orderId, [FromBody] UpdateOrderStatusDTO updateDTO)
        {
            await _orderService.UpdateOrderStatusAsync(orderId, updateDTO.Status);
            return NoContent();
        }

        /// <summary>
        /// Get all orders in the system (Admin only)
        /// </summary>
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PaginatedResult<OrderListDTO>>> GetAllOrders([FromQuery] PaginatedFilterParams filterParams)
        {
            var orders = await _orderService.GetAllOrdersAsync(filterParams);
            return Ok(orders);
        }
    }
}
