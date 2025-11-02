using System;

namespace Application.Queries.Customer
{

    public class GetCustomerByIdQuery
    {
        public Guid Id { get; set; }
    }

    public class GetAllCustomersQuery
    {
        public bool IncludeDeleted { get; set; }
    }
    public class GetCustomerWithOrdersQuery
    {
        public Guid Id { get; set; }
    }
}