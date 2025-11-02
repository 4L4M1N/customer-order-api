using System;

namespace CustomerOrderManagement.Application.Commands.Order
{
    public class CreateOrderCommand
    {
        public Guid CustomerId { get; set; }

    }
}