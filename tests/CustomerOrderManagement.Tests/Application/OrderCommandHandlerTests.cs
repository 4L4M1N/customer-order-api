using CustomerOrderManagement.Application.Commands.Order;
using CustomerOrderManagement.Domain;
using CustomerOrderManagement.Infrastructure.Repositories;
using CustomerOrderManagement.Infrastructure.UnitOfWork;
using Moq;

namespace CustomerOrderManagement.Tests.Application
{
    [TestFixture]
    public class OrderCommandHandlerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ICustomerRepository> _customerRepositoryMock;
        private Mock<IOrderRepository> _orderRepositoryMock;
        private Mock<IProductRepository> _productRepositoryMock;
        private OrderCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();

            _unitOfWorkMock.Setup(u => u.Customers).Returns(_customerRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.Orders).Returns(_orderRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepositoryMock.Object);

            _handler = new OrderCommandHandler(_unitOfWorkMock.Object);
        }

        [Test]
        public async Task HandleCreateOrder_WithValidCustomer_ReturnsOrderId()
        {
            var customerId = Guid.NewGuid();
            var customer = new Customer("John", "Doe", "123 Main St", "12345");
            var command = new CreateOrderCommand { CustomerId = customerId };

            _customerRepositoryMock.Setup(r => r.GetByIdAsync(customerId))
                .ReturnsAsync(customer);
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            var result = await _handler.HandleAsync(command);

            Assert.That(result, Is.Not.EqualTo(Guid.Empty));
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Test]
        public async Task HandleAddOrderItem_WithValidData_AddsItem()
        {
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var order = new Order(Guid.NewGuid());
            var product = new Product("Test Product", 10.00m);

            var command = new AddOrderItemCommand
            {
                OrderId = orderId,
                ProductId = productId,
                Quantity = 2
            };

            _orderRepositoryMock.Setup(r => r.GetByIdWithItemsAsync(orderId))
                .ReturnsAsync(order);
            _productRepositoryMock.Setup(r => r.GetByIdAsync(productId))
                .ReturnsAsync(product);
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            await _handler.HandleAsync(command);

            _orderRepositoryMock.Verify(r => r.Update(order), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }
    }
}