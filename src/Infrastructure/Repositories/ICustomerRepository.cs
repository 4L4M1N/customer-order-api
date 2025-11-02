using CustomerOrderManagement.Domain;

namespace CustomerOrderManagement.Infrastructure.Repositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer> GetByIdWithOrdersAsync(Guid id);
        Task<IEnumerable<Order>> GetCustomerOrdersByDateAsync(Guid customerId, DateTime? startDate, DateTime? endDate);
    }
}