using CustomerOrderManagement.Infrastructure.UnitOfWork;
using System;
using System.Threading.Tasks;

namespace Application.Commands.Product
{
    public class ProductCommandHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> HandleAsync(CreateProductCommand command)
        {
            var product = new CustomerOrderManagement.Domain.Product(command.Name, command.Price);

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();

            return product.Id;
        }

        public async Task HandleAsync(UpdateProductCommand command)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(command.Id);

            if (product == null)
                throw new InvalidOperationException($"Product with ID {command.Id} not found");

            product.UpdateDetails(command.Name, command.Price);

            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();
        }


        public async Task HandleAsync(DeleteProductCommand command)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(command.Id);

            if (product == null)
                throw new InvalidOperationException($"Product with ID {command.Id} not found");

            product.SoftDelete();
            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();
        }
    }
}