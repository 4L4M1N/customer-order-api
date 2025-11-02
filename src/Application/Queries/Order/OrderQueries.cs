using System;

namespace Application.Queries.Order
{
    public class GetOrderByIdQuery
    {
        public Guid Id { get; set; }
    }

    public class GetCustomerOrdersByDateQuery
    {
        public Guid CustomerId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}