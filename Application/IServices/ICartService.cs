using Application.DTOs.Cart;

namespace Application.IServices
{
    public interface ICartService
    {
        Task<CartDTO> GetUserCartAsync(string userId);
        Task<CartDTO> AddItemToCartAsync(string userId, AddCartItemDTO itemDTO);
        Task<CartDTO> UpdateCartItemAsync(string userId, Guid cartItemId, UpdateCartItemDTO updateDTO);
        Task RemoveCartItemAsync(string userId, Guid cartItemId);
        Task ClearCartAsync(string userId);
    }
}
