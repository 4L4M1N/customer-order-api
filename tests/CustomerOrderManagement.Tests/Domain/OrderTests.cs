using CustomerOrderManagement.Domain;

namespace CustomerOrderManagement.Tests.Domain
{
    [TestFixture]
    public class OrderTests
    {
        [Test]
        public void Constructor_CreatesOrderWithCorrectData()
        {
            var customerId = Guid.NewGuid();
            var order = new Order(customerId);
            Assert.That(order.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(order.CustomerId, Is.EqualTo(customerId));
            Assert.That(order.TotalPrice, Is.EqualTo(0));
            Assert.That(order.Items, Is.Empty);
        }

        [Test]
        public void AddItem_WithValidProduct_AddsItemAndCalculatesTotal()
        {
            var order = new Order(Guid.NewGuid());
            var product = new Product("Test Product", 10.50m);
            order.AddItem(product, 2);
            Assert.That(order.Items, Has.Count.EqualTo(1));
            Assert.That(order.TotalPrice, Is.EqualTo(21.00m));
            Assert.That(order.Items.First().Quantity, Is.EqualTo(2));
        }

        [Test]
        public void AddItem_WithSameProductTwice_UpdatesQuantity()
        {
            var order = new Order(Guid.NewGuid());
            var product = new Product("Test Product", 10.00m);
            order.AddItem(product, 2);
            order.AddItem(product, 3);
            Assert.That(order.Items, Has.Count.EqualTo(1));
            Assert.That(order.Items.First().Quantity, Is.EqualTo(5));
            Assert.That(order.TotalPrice, Is.EqualTo(50.00m));
        }

        [Test]
        public void AddItem_WithZeroQuantity_ThrowsArgumentException()
        {
            var order = new Order(Guid.NewGuid());
            var product = new Product("Test Product", 10.00m);
            Assert.Throws<ArgumentException>(() => order.AddItem(product, 0));
        }

        [Test]
        public void RemoveItem_ExistingProduct_RemovesItemAndRecalculatesTotal()
        {
            var order = new Order(Guid.NewGuid());
            var product = new Product("Test Product", 10.00m);
            order.AddItem(product, 2);
            order.RemoveItem(product.Id);
            Assert.That(order.Items, Is.Empty);
            Assert.That(order.TotalPrice, Is.EqualTo(0));
        }

        [Test]
        public void UpdateItemQuantity_WithValidQuantity_UpdatesQuantityAndTotal()
        {
            var order = new Order(Guid.NewGuid());
            var product = new Product("Test Product", 10.00m);
            order.AddItem(product, 2);
            order.UpdateItemQuantity(product.Id, 5);
            Assert.That(order.Items.First().Quantity, Is.EqualTo(5));
            Assert.That(order.TotalPrice, Is.EqualTo(50.00m));
        }

        [Test]
        public void UpdateItemQuantity_NonExistingProduct_ThrowsInvalidOperationException()
        {
            var order = new Order(Guid.NewGuid());
            Assert.Throws<InvalidOperationException>(() =>
                order.UpdateItemQuantity(Guid.NewGuid(), 5));
        }
    }

}