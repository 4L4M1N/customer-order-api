using CustomerOrderManagement.Domain;
using CustomerOrderManagement.Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CustomerOrderManagement.Infrastructure.Repositories
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        public ShoppingCartRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<ShoppingCart> GetByCustomerIdAsync(Guid customerId)
        {
            return await _dbSet
                .Include(c => c.Items)
                .FirstOrDefaultAsync(sc => sc.CustomerId == customerId);
        }

        public async Task<ShoppingCart> GetByCustomerIdWithItemsAsync(Guid customerId)
        {
            return await _dbSet
                .Include(sc => sc.Items)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(sc => sc.CustomerId == customerId);
        }
    }
}