using Application.DTOs.Order;
using Application.IServices;
using Core.Entities;
using Core.Entities.Enums;
using Core.Exceptions;
using Core.IRepositories;
using Shared.Models;

namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;

        public OrderService(IOrderRepository orderRepository, ICartRepository cartRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
        }

        public async Task<OrderDetailDTO> CreateOrderFromCartAsync(string userId, CreateOrderDTO createOrderDTO)
        {
            // Validate shipping address
            if (string.IsNullOrWhiteSpace(createOrderDTO.ShippingAddress))
                throw new BadRequestException("Shipping address is required");

            // Get user's cart
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                throw new BadRequestException("Cart is empty");

            // Calculate total
            decimal totalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.UnitPrice);

            // Create order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTimeOffset.UtcNow,
                TotalAmount = totalAmount,
                Status = OrderStatus.Pending,
                ShippingAddress = createOrderDTO.ShippingAddress,
                PaymentMethod = createOrderDTO.PaymentMethod,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice
                }).ToList()
            };

            var createdOrder = await _orderRepository.CreateOrderAsync(order);

            // Clear cart after successful order creation
            await _cartRepository.ClearCartItemsAsync(cart.Id);

            // Reload order with items to return complete DTO
            var orderWithItems = await _orderRepository.GetOrderWithItemsByIdAsync(createdOrder.Id);

            return MapToOrderDetailDTO(orderWithItems!);
        }

        public async Task<PaginatedResult<OrderListDTO>> GetUserOrdersAsync(string userId, PaginatedFilterParams filterParams)
        {
            var paginatedOrders = await _orderRepository.GetUserOrdersAsync(userId, filterParams);

            var orderListDTOs = paginatedOrders.Items.Select(o => new OrderListDTO
            {
                Id = o.Id,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                PaymentMethod = o.PaymentMethod,
                ItemCount = o.OrderItems?.Count ?? 0
            }).ToList();

            return new PaginatedResult<OrderListDTO>
            {
                Items = orderListDTOs,
                TotalItems = paginatedOrders.TotalItems,
                PageNumber = paginatedOrders.PageNumber,
                PageSize = paginatedOrders.PageSize
            };
        }

        public async Task<OrderDetailDTO> GetOrderByIdAsync(string userId, Guid orderId, bool isAdmin = false)
        {
            var order = await _orderRepository.GetOrderWithItemsByIdAsync(orderId);

            if (order == null)
                throw new NotFoundException($"Order {orderId} not found");

            // Ensure user can only access their own orders and admin can view all
            if (!isAdmin && order.UserId != userId)
                throw new NotFoundException($"Order {orderId} not found");

            return MapToOrderDetailDTO(order);
        }

        public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
                throw new NotFoundException($"Order {orderId} not found");

            // Validate status transition
            ValidateStatusTransition(order.Status, status);

            await _orderRepository.UpdateOrderStatusAsync(orderId, status);
        }

        private void ValidateStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            // Cannot change status if order is already completed or cancelled
            if (currentStatus == OrderStatus.Completed)
                throw new BadRequestException("Cannot change status of a completed order");

            if (currentStatus == OrderStatus.Cancelled)
                throw new BadRequestException("Cannot change status of a cancelled order");

            // Validate allowed transitions
            var allowedTransitions = new Dictionary<OrderStatus, List<OrderStatus>>
            {
                { OrderStatus.Pending, new List<OrderStatus> { OrderStatus.Processing, OrderStatus.Cancelled } },
                { OrderStatus.Processing, new List<OrderStatus> { OrderStatus.Shipped, OrderStatus.Cancelled } },
                { OrderStatus.Shipped, new List<OrderStatus> { OrderStatus.Completed, OrderStatus.Cancelled } }
            };

            if (allowedTransitions.ContainsKey(currentStatus))
            {
                if (!allowedTransitions[currentStatus].Contains(newStatus))
                {
                    throw new BadRequestException(
                        $"Invalid status transition from {currentStatus} to {newStatus}. " +
                        $"Allowed transitions: {string.Join(", ", allowedTransitions[currentStatus])}"
                    );
                }
            }

            // Prevent moving backwards in the workflow (except cancellation)
            if (newStatus != OrderStatus.Cancelled && newStatus < currentStatus)
            {
                throw new BadRequestException($"Cannot move order status backwards from {currentStatus} to {newStatus}");
            }
        }

        public async Task<PaginatedResult<OrderListDTO>> GetAllOrdersAsync(PaginatedFilterParams filterParams)
        {
            var paginatedOrders = await _orderRepository.GetAllOrdersAsync(filterParams);

            var orderListDTOs = paginatedOrders.Items.Select(o => new OrderListDTO
            {
                Id = o.Id,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                PaymentMethod = o.PaymentMethod,
                ItemCount = o.OrderItems?.Count ?? 0
            }).ToList();

            return new PaginatedResult<OrderListDTO>
            {
                Items = orderListDTOs,
                TotalItems = paginatedOrders.TotalItems,
                PageNumber = paginatedOrders.PageNumber,
                PageSize = paginatedOrders.PageSize
            };
        }

        private OrderDetailDTO MapToOrderDetailDTO(Order order)
        {
            return new OrderDetailDTO
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                ShippingAddress = order.ShippingAddress,
                PaymentMethod = order.PaymentMethod,
                OrderItems = order.OrderItems?.Select(oi => new OrderItemDTO
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? string.Empty,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList() ?? new List<OrderItemDTO>()
            };
        }
    }
}
