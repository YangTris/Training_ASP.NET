using System;

namespace Application.DTOs.Order;

public class OrderDetailDTO
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTimeOffset OrderDate { get; set; }
}
