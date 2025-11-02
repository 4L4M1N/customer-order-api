using CustomerOrderManagement.Infrastructure.UnitOfWork;
using System;
using System.Threading.Tasks;

namespace CustomerOrderManagement.Application.Commands.Customer
{
    public class CustomerCommandHandler
    {

        private readonly IUnitOfWork _unitOfWork;

        public CustomerCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> HandleAsync(CreateCustomerCommand command)
        {
            var customer = new Domain.Customer(
                command.FirstName,
                command.LastName,
                command.Address,
                command.PostalCode
            );

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.CompleteAsync();

            return customer.Id;
        }

        public async Task HandleAsync(UpdateCustomerCommand command)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(command.Id);

            if (customer == null)
                throw new InvalidOperationException($"Customer with ID {command.Id} not found");

            customer.UpdateDetails(
                command.FirstName,
                command.LastName,
                command.Address,
                command.PostalCode
            );

            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.CompleteAsync();
        }

        public async Task HandleAsync(DeleteCustomerCommand command)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(command.Id);

            if (customer == null)
                throw new InvalidOperationException($"Customer with ID {command.Id} not found");

            customer.SoftDelete();
            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.CompleteAsync();
        }

    }
}