using System;

namespace CustomerOrderManagement.Application.Queries.ShoppingCart
{
    public class ShoppingCartQueries
    {
        public class GetShoppingCartQuery
        {
            public Guid CustomerId { get; set; }
        }
    }
}