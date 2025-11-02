using CustomerOrderManagement.Infrastructure.UnitOfWork;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerOrderManagement.Application.Commands.ShoppingCart
{
    public class ShoppingCartCommandHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(ShoppingCartCommands.AddToCartCommand command)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(command.ProductId);
            if (product == null || product.IsDeleted)
                throw new InvalidOperationException($"Product with ID {command.ProductId} not found");

            if (_unitOfWork.Customers != null)
            {
                var customer = await _unitOfWork.Customers.GetByIdAsync(command.CustomerId);
                if (customer == null || customer.IsDeleted)
                    throw new InvalidOperationException($"Customer with ID {command.CustomerId} not found");
            }

            var cart = await _unitOfWork.ShoppingCarts.GetByCustomerIdWithItemsAsync(command.CustomerId);

            if (cart == null)
            {
                cart = new Domain.ShoppingCart(command.CustomerId);
                cart.AddItem(product, command.Quantity);
                await _unitOfWork.ShoppingCarts.AddAsync(cart);
            }
            else
            {
                var existingItem = cart.Items.Any(i => i.ProductId == product.Id);
                cart.AddItem(product, command.Quantity);

                if (!existingItem)
                {
                    var newItem = cart.Items.First(i => i.ProductId == product.Id);
                    if (_unitOfWork.CartItems != null)
                    {
                        await _unitOfWork.CartItems.AddAsync(newItem);
                    }
                }
                _unitOfWork.ShoppingCarts.Update(cart);
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task HandleAsync(ShoppingCartCommands.UpdateCartItemCommand command)
        {
            var cart = await _unitOfWork.ShoppingCarts.GetByCustomerIdWithItemsAsync(command.CustomerId);

            if (cart == null)
                throw new InvalidOperationException($"Shopping cart not found for customer {command.CustomerId}");

            cart.UpdateItemQuantity(command.ProductId, command.Quantity);
            _unitOfWork.ShoppingCarts.Update(cart);
            await _unitOfWork.CompleteAsync();
        }

        public async Task HandleAsync(ShoppingCartCommands.RemoveFromCartCommand command)
        {
            var cart = await _unitOfWork.ShoppingCarts.GetByCustomerIdWithItemsAsync(command.CustomerId);
            if (cart == null)
                throw new InvalidOperationException($"Shopping cart not found for customer {command.CustomerId}");

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == command.ProductId);
            if (existingItem == null)
                return;

            cart.RemoveItem(command.ProductId);
            _unitOfWork.CartItems?.Remove(existingItem);
            _unitOfWork.ShoppingCarts.Update(cart);
            await _unitOfWork.CompleteAsync();
        }

        public async Task HandleAsync(ShoppingCartCommands.ClearCartCommand command)
        {
            var cart = await _unitOfWork.ShoppingCarts.GetByCustomerIdWithItemsAsync(command.CustomerId);

            if (cart == null)
                return;

            foreach (var item in cart.Items.ToList())
            {
                _unitOfWork.CartItems?.Remove(item);
            }
            cart.Clear();
            _unitOfWork.ShoppingCarts.Update(cart);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<Guid> HandleAsync(ShoppingCartCommands.CheckoutCommand command)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var cart = await _unitOfWork.ShoppingCarts.GetByCustomerIdWithItemsAsync(command.CustomerId);

                if (cart == null)
                    throw new InvalidOperationException($"Shopping cart not found for customer {command.CustomerId}");

                if (cart.IsEmpty())
                    throw new InvalidOperationException("Cannot checkout with an empty cart");

                var order = cart.ConvertToOrder();
                await _unitOfWork.Orders.AddAsync(order);
                _unitOfWork.ShoppingCarts.Remove(cart);
                await _unitOfWork.CommitTransactionAsync();

                return order.Id;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}