using CustomerOrderManagement.Domain;
using CustomerOrderManagement.Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CustomerOrderManagement.Infrastructure.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<Customer> GetByIdWithOrdersAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.Orders)
                    .ThenInclude(o => o.Items)
                        .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Order>> GetCustomerOrdersByDateAsync(Guid customerId, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Where(o => o.CustomerId == customerId);

            if (startDate.HasValue)
                query = query.Where(o => o.CreatedOnUtc >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(o => o.CreatedOnUtc <= endDate.Value);

            return await query.OrderBy(o => o.CreatedOnUtc).ToListAsync();
        }
    }
}