using CustomerOrderManagement.Application.Commands.Customer;
using CustomerOrderManagement.Domain;
using CustomerOrderManagement.Infrastructure.Repositories;
using CustomerOrderManagement.Infrastructure.UnitOfWork;
using Moq;

namespace CustomerOrderManagement.Tests.Application
{
    [TestFixture]
    public class CustomerCommandHandlerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ICustomerRepository> _customerRepositoryMock;
        private CustomerCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _unitOfWorkMock.Setup(u => u.Customers).Returns(_customerRepositoryMock.Object);
            _handler = new CustomerCommandHandler(_unitOfWorkMock.Object);
        }

        [Test]
        public async Task HandleCreateCustomer_WithValidCommand_ReturnsCustomerId()
        {
            var command = new CreateCustomerCommand
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Address = "123 Main Address",
                PostalCode = "12345"
            };

            _customerRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Customer>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            var result = await _handler.HandleAsync(command);

            Assert.That(result, Is.Not.EqualTo(Guid.Empty));
            _customerRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Test]
        public async Task HandleUpdateCustomer_WithNonExistingCustomer_ThrowsInvalidOperationException()
        {
            var command = new UpdateCustomerCommand
            {
                Id = Guid.NewGuid(),
                FirstName = "FirstName",
                LastName = "LastName",
                Address = "123 Main Address",
                PostalCode = "12345"
            };

            _customerRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Customer)null);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _handler.HandleAsync(command));
        }

        [Test]
        public async Task HandleDeleteCustomer_WithExistingCustomer_DeletesCustomer()
        {
            var customerId = Guid.NewGuid();
            var customer = new Customer("FirstName", "LastName", "123 Main Address", "12345");
            var command = new DeleteCustomerCommand { Id = customerId };

            _customerRepositoryMock.Setup(r => r.GetByIdAsync(customerId))
                .ReturnsAsync(customer);
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
           
            await _handler.HandleAsync(command);
           
            Assert.That(customer.IsDeleted, Is.True, "Customer should be marked as deleted");
            Assert.That(customer.DeletedOnUtc, Is.Not.Null, "DeletedOnUtc should be set");

            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
            _customerRepositoryMock.Verify(r => r.Remove(It.IsAny<Customer>()), Times.Never);
        }
    }
}