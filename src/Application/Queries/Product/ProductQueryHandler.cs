using Application.DTOs;
using CustomerOrderManagement.Infrastructure.UnitOfWork;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Queries.Product
{
    public class ProductQueryHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductDto> HandleAsync(GetProductByIdQuery query)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(query.Id);

            if (product == null)
                return null;

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price
            };
        }

        public async Task<IEnumerable<ProductDto>> HandleAsync(GetAllProductsQuery query)
        {
            var products = await _unitOfWork.Products.GetAllAsync();

            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            });
        }
    }
}