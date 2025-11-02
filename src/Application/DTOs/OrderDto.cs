using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}