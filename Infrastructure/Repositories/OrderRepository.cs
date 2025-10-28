using Core.Entities;
using Core.Entities.Enums;
using Core.IRepositories;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<PaginatedResult<Order>> GetUserOrdersAsync(string userId, PaginatedFilterParams filterParams)
        {
            IQueryable<Order> query = _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId);

            query = filterParams.SortBy?.ToLower() switch
            {
                "totalamount" => filterParams.IsDescending
                    ? query.OrderByDescending(o => o.TotalAmount)
                    : query.OrderBy(o => o.TotalAmount),
                "status" => filterParams.IsDescending
                    ? query.OrderByDescending(o => o.Status)
                    : query.OrderBy(o => o.Status),
                _ => filterParams.IsDescending
                    ? query.OrderByDescending(o => o.OrderDate)
                    : query.OrderBy(o => o.OrderDate),
            };

            var total = await query.CountAsync();
            var items = await query
                .Skip((filterParams.PageNumber - 1) * filterParams.PageSize)
                .Take(filterParams.PageSize)
                .ToListAsync();

            return new PaginatedResult<Order>
            {
                Items = items,
                TotalItems = total,
                PageNumber = filterParams.PageNumber,
                PageSize = filterParams.PageSize
            };
        }

        public async Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<Order?> GetOrderWithItemsByIdAsync(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(PaginatedFilterParams filterParams)
        {
            IQueryable<Order> query = _context.Orders
                .Include(o => o.OrderItems);

            query = filterParams.SortBy?.ToLower() switch
            {
                "totalamount" => filterParams.IsDescending
                    ? query.OrderByDescending(o => o.TotalAmount)
                    : query.OrderBy(o => o.TotalAmount),
                "status" => filterParams.IsDescending
                    ? query.OrderByDescending(o => o.Status)
                    : query.OrderBy(o => o.Status),
                _ => filterParams.IsDescending
                    ? query.OrderByDescending(o => o.OrderDate)
                    : query.OrderBy(o => o.OrderDate),
            };

            return await query
                .Skip((filterParams.PageNumber - 1) * filterParams.PageSize)
                .Take(filterParams.PageSize)
                .ToListAsync();
        }
    }
}
