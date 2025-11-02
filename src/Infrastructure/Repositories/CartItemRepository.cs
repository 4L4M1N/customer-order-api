using CustomerOrderManagement.Domain;
using CustomerOrderManagement.Infrastructure.Data;
using Infrastructure.Repositories;

namespace CustomerOrderManagement.Infrastructure.Repositories
{
    public class CartItemRepository : Repository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(AppDbContext context) : base(context)
        {
        }
    }
}