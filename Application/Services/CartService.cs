using Application.DTOs.Cart;
using Application.IServices;
using Core.Entities;
using Core.Exceptions;
using Core.IRepositories;

namespace Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<CartDTO> GetUserCartAsync(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            // Return empty cart if none exists
            if (cart == null)
            {
                return new CartDTO
                {
                    Id = Guid.Empty,
                    UserId = userId,
                    Items = new List<CartItemDTO>()
                };
            }

            return MapToCartDTO(cart);
        }

        public async Task<CartDTO> AddItemToCartAsync(string userId, AddCartItemDTO itemDTO)
        {
            if (itemDTO.Quantity <= 0)
                throw new BadRequestException("Quantity must be greater than 0");

            var product = await _productRepository.GetByIdAsync(itemDTO.ProductId);
            if (product == null)
                throw new NotFoundException($"Product {itemDTO.ProductId} not found");

            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                cart = await _cartRepository.CreateCartAsync(cart);
            }

            // Check if product already in cart
            var existingItem = cart.CartItems?.FirstOrDefault(ci => ci.ProductId == itemDTO.ProductId);

            if (existingItem != null)
            {
                // Update quantity
                existingItem.Quantity += itemDTO.Quantity;
                await _cartRepository.UpdateCartItemAsync(existingItem);
            }
            else
            {
                // Add new item
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = itemDTO.ProductId,
                    Quantity = itemDTO.Quantity,
                    UnitPrice = product.Price
                };
                await _cartRepository.AddCartItemAsync(cartItem);
            }

            // Reload cart with updated items
            cart = await _cartRepository.GetCartByUserIdAsync(userId);
            return MapToCartDTO(cart!);
        }

        public async Task<CartDTO> UpdateCartItemAsync(string userId, Guid cartItemId, UpdateCartItemDTO updateDTO)
        {
            if (updateDTO.Quantity <= 0)
                throw new BadRequestException("Quantity must be greater than 0");

            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
                throw new NotFoundException("Cart not found");

            var cartItem = cart.CartItems?.FirstOrDefault(ci => ci.Id == cartItemId);
            if (cartItem == null)
                throw new NotFoundException($"Cart item {cartItemId} not found");

            cartItem.Quantity = updateDTO.Quantity;
            await _cartRepository.UpdateCartItemAsync(cartItem);

            // Reload cart
            cart = await _cartRepository.GetCartByUserIdAsync(userId);
            return MapToCartDTO(cart!);
        }

        public async Task RemoveCartItemAsync(string userId, Guid cartItemId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
                throw new NotFoundException("Cart not found");

            var cartItem = cart.CartItems?.FirstOrDefault(ci => ci.Id == cartItemId);
            if (cartItem == null)
                throw new NotFoundException($"Cart item {cartItemId} not found");

            await _cartRepository.RemoveCartItemAsync(cartItem);
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
                throw new NotFoundException("Cart not found");

            if (cart.CartItems != null && cart.CartItems.Any())
            {
                await _cartRepository.ClearCartItemsAsync(cart.Id);
            }
        }

        private CartDTO MapToCartDTO(Cart cart)
        {
            return new CartDTO
            {
                Id = cart.Id,
                UserId = cart.UserId,
                Items = cart.CartItems?.Select(ci => new CartItemDTO
                {
                    Id = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product?.Name ?? string.Empty,
                    UnitPrice = ci.UnitPrice,
                    Quantity = ci.Quantity
                }).ToList() ?? new List<CartItemDTO>()
            };
        }
    }
}
