using System;

namespace CustomerOrderManagement.Application.Commands.Customer
{
    public class UpdateCustomerCommand
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }

    }
}