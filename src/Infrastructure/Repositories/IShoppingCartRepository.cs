using CustomerOrderManagement.Domain;

namespace CustomerOrderManagement.Infrastructure.Repositories
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        Task<ShoppingCart> GetByCustomerIdAsync(Guid customerId);
        Task<ShoppingCart> GetByCustomerIdWithItemsAsync(Guid customerId);
    }
}