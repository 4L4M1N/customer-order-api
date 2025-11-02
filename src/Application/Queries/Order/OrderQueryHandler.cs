using Application.DTOs;
using CustomerOrderManagement.Infrastructure.UnitOfWork;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Queries.Order
{
    public class OrderQueryHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderDto> HandleAsync(GetOrderByIdQuery query)
        {
            var order = await _unitOfWork.Orders.GetByIdWithItemsAsync(query.Id);

            if (order == null)
                return null;

            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                OrderDate = order.CreatedOnUtc,
                TotalPrice = order.TotalPrice,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    LineTotal = i.ItemTotal
                }).ToList()
            };
        }

        public async Task<IEnumerable<OrderDto>> HandleAsync(GetCustomerOrdersByDateQuery query)
        {
            var orders = await _unitOfWork.Customers.GetCustomerOrdersByDateAsync(
                query.CustomerId,
                query.StartDate,
                query.EndDate
            );

            return orders.Select(o => new OrderDto
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
            });
        }
    }
}