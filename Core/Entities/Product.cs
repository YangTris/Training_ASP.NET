using System;
using Core.Exceptions;

namespace Core.Entities
{

    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Foreign Keys
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }

        // Navigation
        public ICollection<ProductImage>? Images { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }

        // public void SetProduct(string name, string description, decimal price)
        // {
        //     if (string.IsNullOrWhiteSpace(name))
        //         throw new DomainException("Product name cannot be empty.");
        //     if (price < 0)
        //         throw new DomainException("Product price cannot be negative.");
        //     Name = name;
        //     Description = description;
        //     Price = price;
        //     UpdatedAt = DateTimeOffset.UtcNow;
        // }
    }
}
