using System;

namespace CustomerOrderManagement.Application.Commands.ShoppingCart
{
    public class ShoppingCartCommands
    {
        public class AddToCartCommand
        {
            public Guid CustomerId { get; set; }
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
        }
        public class UpdateCartItemCommand
        {
            public Guid CustomerId { get; set; }
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
        }

        public class RemoveFromCartCommand
        {
            public Guid CustomerId { get; set; }
            public Guid ProductId { get; set; }
        }

        public class ClearCartCommand
        {
            public Guid CustomerId { get; set; }
        }

        public class CheckoutCommand
        {
            public Guid CustomerId { get; set; }
        }
    }
}