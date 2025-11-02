using CustomerOrderManagement.Application.Commands.ShoppingCart;
using CustomerOrderManagement.Domain;
using CustomerOrderManagement.Infrastructure.Repositories;
using CustomerOrderManagement.Infrastructure.UnitOfWork;
using Moq;

namespace CustomerOrderManagement.Tests.Application
{
    [TestFixture]
    public class ShoppingCartCommandHandlerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IShoppingCartRepository> _cartRepositoryMock;
        private Mock<IProductRepository> _productRepositoryMock;
        private Mock<IOrderRepository> _orderRepositoryMock;
        private ShoppingCartCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _cartRepositoryMock = new Mock<IShoppingCartRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _orderRepositoryMock = new Mock<IOrderRepository>();

            _unitOfWorkMock.Setup(u => u.ShoppingCarts).Returns(_cartRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.Orders).Returns(_orderRepositoryMock.Object);

            _handler = new ShoppingCartCommandHandler(_unitOfWorkMock.Object);
        }

        [Test]
        public async Task HandleAddToCart_CreatesNewCart_WhenCartDoesNotExist()
        {
            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var product = new Product("Test Product", 10.00m);

            var command = new ShoppingCartCommands.AddToCartCommand
            {
                CustomerId = customerId,
                ProductId = productId,
                Quantity = 2
            };

            _cartRepositoryMock.Setup(r => r.GetByCustomerIdWithItemsAsync(customerId))
                .ReturnsAsync((ShoppingCart)null);
            _productRepositoryMock.Setup(r => r.GetByIdAsync(productId))
                .ReturnsAsync(product);
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            
            await _handler.HandleAsync(command);
            
            _cartRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ShoppingCart>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Test]
        public async Task HandleAddToCart_AddsToExistingCart_WhenCartExists()
        {
            
            var customerId = Guid.NewGuid();
            var cart = new ShoppingCart(customerId);
            var product = new Product("Test Product", 10.00m);

            var command = new ShoppingCartCommands.AddToCartCommand
            {
                CustomerId = customerId,
                ProductId = product.Id,
                Quantity = 2
            };

            _cartRepositoryMock.Setup(r => r.GetByCustomerIdWithItemsAsync(customerId))
                .ReturnsAsync(cart);
            _productRepositoryMock.Setup(r => r.GetByIdAsync(product.Id))
                .ReturnsAsync(product);
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            
            await _handler.HandleAsync(command);
            
            _cartRepositoryMock.Verify(r => r.Update(cart), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Test]
        public void HandleAddToCart_ThrowsException_WhenProductNotFound()
        {
            var command = new ShoppingCartCommands.AddToCartCommand
            {
                CustomerId = Guid.NewGuid(),
                ProductId = Guid.NewGuid(),
                Quantity = 2
            };

            _cartRepositoryMock.Setup(r => r.GetByCustomerIdWithItemsAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ShoppingCart)null);
            _productRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Product)null);
            
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _handler.HandleAsync(command));
        }

        [Test]
        public async Task HandleCheckout_CreatesOrderAndDeletesCart()
        {
            
            var customerId = Guid.NewGuid();
            var cart = new ShoppingCart(customerId);
            cart.AddItem(new Product("Test", 10.00m), 2);

            var command = new ShoppingCartCommands.CheckoutCommand { CustomerId = customerId };

            _cartRepositoryMock.Setup(r => r.GetByCustomerIdWithItemsAsync(customerId))
                .ReturnsAsync(cart);
            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);
            
            var orderId = await _handler.HandleAsync(command);
            
            Assert.That(orderId, Is.Not.EqualTo(Guid.Empty));
            _orderRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
            _cartRepositoryMock.Verify(r => r.Remove(cart), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }

        [Test]
        public void HandleCheckout_ThrowsException_WhenCartIsEmpty()
        {
            
            var customerId = Guid.NewGuid();
            var cart = new ShoppingCart(customerId);

            var command = new ShoppingCartCommands.CheckoutCommand { CustomerId = customerId };

            _cartRepositoryMock.Setup(r => r.GetByCustomerIdWithItemsAsync(customerId))
                .ReturnsAsync(cart);
            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _handler.HandleAsync(command));
            _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
        }

        [Test]
        public async Task HandleRemoveFromCart_RemovesItem()
        {
            var customerId = Guid.NewGuid();
            var cart = new ShoppingCart(customerId);
            var product = new Product("Test", 10.00m);
            cart.AddItem(product, 2);

            var command = new ShoppingCartCommands.RemoveFromCartCommand
            {
                CustomerId = customerId,
                ProductId = product.Id
            };

            _cartRepositoryMock.Setup(r => r.GetByCustomerIdWithItemsAsync(customerId))
                .ReturnsAsync(cart);
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            
            await _handler.HandleAsync(command);
            
            Assert.That(cart.Items, Is.Empty);
            _cartRepositoryMock.Verify(r => r.Update(cart), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Test]
        public async Task HandleClearCart_ClearsAllItems()
        {
            
            var customerId = Guid.NewGuid();
            var cart = new ShoppingCart(customerId);
            cart.AddItem(new Product("Item 1", 10.00m), 2);
            cart.AddItem(new Product("Item 2", 20.00m), 1);

            var command = new ShoppingCartCommands.ClearCartCommand { CustomerId = customerId };

            _cartRepositoryMock.Setup(r => r.GetByCustomerIdWithItemsAsync(customerId))
                .ReturnsAsync(cart);
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            
            await _handler.HandleAsync(command);
            
            Assert.That(cart.Items, Is.Empty);
            Assert.That(cart.TotalPrice, Is.EqualTo(0));
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }
    }
}