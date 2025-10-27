using Application.DTOs.Order;

namespace Application.DTOs.User
{
    public class UserDetailDTO : UserListDTO
    {
        public string? PhoneNumber { get; set; }
        public List<OrderDetailDTO> OrderDetailDTOs = new();
    }
}