using System;
using System.Collections.Generic;

namespace CustomerOrderManagement.Application.DTOs
{
    public class ShoppingCartDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int ItemCount { get; set; }
        public List<CartItemDto> Items { get; set; }
    }

    public class CartItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}