using CustomerOrderManagement.Domain;
using CustomerOrderManagement.Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CustomerOrderManagement.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Order> GetByIdWithItemsAsync(Guid id)
        {
            return await _dbSet
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}