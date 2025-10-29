using Core.Entities;
using Core.Entities.Enums;
using Shared.Models;

namespace Core.IRepositories
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<PaginatedResult<Order>> GetUserOrdersAsync(string userId, PaginatedFilterParams filterParams);
        Task<Order?> GetOrderByIdAsync(Guid orderId);
        Task<Order?> GetOrderWithItemsByIdAsync(Guid orderId);
        Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status);
        Task<PaginatedResult<Order>> GetAllOrdersAsync(PaginatedFilterParams filterParams);
    }
}
