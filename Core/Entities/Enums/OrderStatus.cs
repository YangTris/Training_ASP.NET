using System;

namespace Core.Entities.Enums;

public enum OrderStatus
{
    Pending = 0,
    Processing = 1,
    Shipped = 2,
    Completed = 3,
    Cancelled = 4
}
