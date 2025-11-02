using CustomerOrderManagement.Application.DTOs;
using CustomerOrderManagement.Infrastructure.UnitOfWork;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerOrderManagement.Application.Queries.ShoppingCart
{
    public class ShoppingCartQueryHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ShoppingCartDto> HandleAsync(ShoppingCartQueries.GetShoppingCartQuery query)
        {
            var cart = await _unitOfWork.ShoppingCarts.GetByCustomerIdWithItemsAsync(query.CustomerId);

            if (cart == null)
                return null;

            return new ShoppingCartDto
            {
                Id = cart.Id,
                CustomerId = cart.CustomerId,
                CreatedDate = cart.CreatedOnUtc,
                LastModifiedDate = cart.UpdatedOnUtc,
                TotalPrice = cart.TotalPrice,
                ItemCount = cart.Items.Count,
                Items = cart.Items.Select(ci => new CartItemDto
                {
                    Id = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product?.Name,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice,
                    LineTotal = ci.ItemTotal
                }).ToList()
            };
        }
    }
}