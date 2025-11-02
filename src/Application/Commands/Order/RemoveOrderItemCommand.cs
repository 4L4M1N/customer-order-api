using System;

namespace CustomerOrderManagement.Application.Commands.Order
{
    public class RemoveOrderItemCommand
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }

    }
}