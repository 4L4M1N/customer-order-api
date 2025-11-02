using CustomerOrderManagement.Infrastructure.Repositories;

namespace CustomerOrderManagement.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ICustomerRepository Customers { get; }

        IOrderRepository Orders { get; }

        IProductRepository Products { get; }

        IShoppingCartRepository ShoppingCarts { get; }

        ICartItemRepository CartItems { get; }

        Task<int> CompleteAsync();

        Task BeginTransactionAsync();

        Task CommitTransactionAsync();

        Task RollbackTransactionAsync();
    }
}