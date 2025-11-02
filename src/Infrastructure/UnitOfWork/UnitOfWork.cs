using CustomerOrderManagement.Infrastructure.Data;
using CustomerOrderManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace CustomerOrderManagement.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _transaction;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Customers = new CustomerRepository(_context);
            Orders = new OrderRepository(_context);
            Products = new ProductRepository(_context);
            ShoppingCarts = new ShoppingCartRepository(_context);
            CartItems = new CartItemRepository(_context);
        }

        public ICustomerRepository Customers { get; private set; }
        public IOrderRepository Orders { get; private set; }
        public IProductRepository Products { get; private set; }

        public IShoppingCartRepository ShoppingCarts { get; private set; }

        public ICartItemRepository CartItems { get; private set; }


        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}