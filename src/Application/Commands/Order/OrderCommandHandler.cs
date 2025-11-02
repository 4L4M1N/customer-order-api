using CustomerOrderManagement.Infrastructure.UnitOfWork;
using System;
using System.Threading.Tasks;

namespace CustomerOrderManagement.Application.Commands.Order
{
    public class OrderCommandHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> HandleAsync(CreateOrderCommand command)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(command.CustomerId);

            if (customer == null)
                throw new InvalidOperationException($"Customer with ID {command.CustomerId} not found");

            var order = customer.CreateOrder();

            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.CompleteAsync();

            return order.Id;
        }

        public async Task HandleAsync(AddOrderItemCommand command)
        {
            var order = await _unitOfWork.Orders.GetByIdWithItemsAsync(command.OrderId);

            if (order == null)
                throw new InvalidOperationException($"Order with ID {command.OrderId} not found");

            var product = await _unitOfWork.Products.GetByIdAsync(command.ProductId);

            if (product == null)
                throw new InvalidOperationException($"Product with ID {command.ProductId} not found");

            order.AddItem(product, command.Quantity);

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();
        }

        public async Task HandleAsync(UpdateOrderItemCommand command)
        {
            var order = await _unitOfWork.Orders.GetByIdWithItemsAsync(command.OrderId);

            if (order == null)
                throw new InvalidOperationException($"Order with ID {command.OrderId} not found");

            order.UpdateItemQuantity(command.ProductId, command.Quantity);

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();
        }

        public async Task HandleAsync(RemoveOrderItemCommand command)
        {
            var order = await _unitOfWork.Orders.GetByIdWithItemsAsync(command.OrderId);

            if (order == null)
                throw new InvalidOperationException($"Order with ID {command.OrderId} not found");

            order.RemoveItem(command.ProductId);

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();
        }

        public async Task HandleAsync(DeleteOrderCommand command)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(command.OrderId);

            if (order == null)
                throw new InvalidOperationException($"Order with ID {command.OrderId} not found");

            _unitOfWork.Orders.Remove(order);
            await _unitOfWork.CompleteAsync();
        }
    }
}