using Application.DTOs;
using CustomerOrderManagement.Infrastructure.UnitOfWork;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Queries.Customer
{
    public class CustomerQueryHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerDto> HandleAsync(GetCustomerByIdQuery query)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(query.Id);

            if (customer == null)
                return null;

            return new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Address = customer.Address,
                PostalCode = customer.PostalCode
            };
        }

        public async Task<IEnumerable<CustomerDto>> HandleAsync(GetAllCustomersQuery query)
        {
            IEnumerable<CustomerOrderManagement.Domain.Customer> customers;

            if (query.IncludeDeleted)
                customers = await _unitOfWork.Customers.GetAllAsync();
            else
                customers = await _unitOfWork.Customers.FindAsync(c => !c.IsDeleted);

            return customers.Select(c => new CustomerDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Address = c.Address,
                PostalCode = c.PostalCode
            });
        }

        public async Task<CustomerDetailDto> HandleAsync(GetCustomerWithOrdersQuery query)
        {
            var customer = await _unitOfWork.Customers.GetByIdWithOrdersAsync(query.Id);

            if (customer == null)
                return null;

            return new CustomerDetailDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Address = customer.Address,
                PostalCode = customer.PostalCode,
                Orders = customer.Orders.Select(o => new OrderDto
                {
                    Id = o.Id,
                    CustomerId = o.CustomerId,
                    OrderDate = o.CreatedOnUtc,
                    TotalPrice = o.TotalPrice,
                    Items = o.Items.Select(i => new OrderItemDto
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ProductName = i.Product?.Name,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        LineTotal = i.ItemTotal
                    }).ToList()
                }).ToList()
            };
        }
    }
}