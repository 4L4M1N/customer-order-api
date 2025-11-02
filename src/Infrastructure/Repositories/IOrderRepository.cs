using CustomerOrderManagement.Domain;

namespace CustomerOrderManagement.Infrastructure.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order> GetByIdWithItemsAsync(Guid id);
    }
}