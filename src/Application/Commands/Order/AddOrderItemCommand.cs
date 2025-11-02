using System;

namespace CustomerOrderManagement.Application.Commands.Order
{
    public class AddOrderItemCommand
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

    }
}