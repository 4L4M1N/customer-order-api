using CustomerOrderManagement.Domain;
using CustomerOrderManagement.Infrastructure.Data;
using Infrastructure.Repositories;

namespace CustomerOrderManagement.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }
    }
}