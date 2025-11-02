using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    public class CustomerDetailDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public List<OrderDto> Orders { get; set; }
    }
}