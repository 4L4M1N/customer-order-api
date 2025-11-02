using System;

namespace CustomerOrderManagement.Application.Commands.Order
{
    public class DeleteOrderCommand
    {
        public Guid OrderId { get; set; }

    }
}