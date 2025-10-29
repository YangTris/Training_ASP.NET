using Application.DTOs.Order;
using Core.Entities.Enums;
using Shared.Models;

namespace Application.IServices
{
    public interface IOrderService
    {
        Task<OrderDetailDTO> CreateOrderFromCartAsync(string userId, CreateOrderDTO createOrderDTO);
        Task<PaginatedResult<OrderListDTO>> GetUserOrdersAsync(string userId, PaginatedFilterParams filterParams);
        Task<OrderDetailDTO> GetOrderByIdAsync(string userId, Guid orderId);
        Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status);
        Task<PaginatedResult<OrderListDTO>> GetAllOrdersAsync(PaginatedFilterParams filterParams);
    }
}
