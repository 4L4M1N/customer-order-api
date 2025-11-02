using CustomerOrderManagement.Domain;

namespace CustomerOrderManagement.Tests.Domain
{
    [TestFixture]
    public class ShoppingCartTests
    {
        [Test]
        public void Constructor_CreatesEmptyCart()
        {
            var customerId = Guid.NewGuid();
            var cart = new ShoppingCart(customerId);
            Assert.That(cart.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(cart.CustomerId, Is.EqualTo(customerId));
            Assert.That(cart.TotalPrice, Is.EqualTo(0));
            Assert.That(cart.Items, Is.Empty);
            Assert.That(cart.IsEmpty(), Is.True);
        }

        [Test]
        public void AddItem_WithNewProduct_AddsItemToCart()
        {
            var cart = new ShoppingCart(Guid.NewGuid());
            var product = new Product("Laptop", 999.99m);
            cart.AddItem(product, 2);
            Assert.That(cart.Items, Has.Count.EqualTo(1));
            Assert.That(cart.TotalPrice, Is.EqualTo(1999.98m));
            Assert.That(cart.IsEmpty(), Is.False);
        }

        [Test]
        public void AddItem_WithSameProductTwice_IncreasesQuantity()
        {
            var cart = new ShoppingCart(Guid.NewGuid());
            var product = new Product("Mouse", 25.00m);
            cart.AddItem(product, 2);
            cart.AddItem(product, 3);
            Assert.That(cart.Items, Has.Count.EqualTo(1));
            Assert.That(cart.Items.First().Quantity, Is.EqualTo(5));
            Assert.That(cart.TotalPrice, Is.EqualTo(125.00m));
        }

        [Test]
        public void AddItem_WithZeroQuantity_ThrowsArgumentException()
        {
            var cart = new ShoppingCart(Guid.NewGuid());
            var product = new Product("Keyboard", 50.00m);
            Assert.Throws<ArgumentException>(() => cart.AddItem(product, 0));
        }

        [Test]
        public void UpdateItemQuantity_WithValidQuantity_UpdatesItem()
        {
            var cart = new ShoppingCart(Guid.NewGuid());
            var product = new Product("Monitor", 300.00m);
            cart.AddItem(product, 2);
            cart.UpdateItemQuantity(product.Id, 5);
            Assert.That(cart.Items.First().Quantity, Is.EqualTo(5));
            Assert.That(cart.TotalPrice, Is.EqualTo(1500.00m));
        }

        [Test]
        public void UpdateItemQuantity_WithNonExistingProduct_ThrowsInvalidOperationException()
        {
            var cart = new ShoppingCart(Guid.NewGuid());
            Assert.Throws<InvalidOperationException>(() =>
                cart.UpdateItemQuantity(Guid.NewGuid(), 5));
        }

        [Test]
        public void RemoveItem_ExistingProduct_RemovesItemAndRecalculatesTotal()
        {
            var cart = new ShoppingCart(Guid.NewGuid());
            var product1 = new Product("Item 1", 10.00m);
            var product2 = new Product("Item 2", 20.00m);
            cart.AddItem(product1, 1);
            cart.AddItem(product2, 1);
            cart.RemoveItem(product1.Id);
            Assert.That(cart.Items, Has.Count.EqualTo(1));
            Assert.That(cart.TotalPrice, Is.EqualTo(20.00m));
        }

        [Test]
        public void Clear_RemovesAllItems()
        {
            var cart = new ShoppingCart(Guid.NewGuid());
            cart.AddItem(new Product("Item 1", 10.00m), 1);
            cart.AddItem(new Product("Item 2", 20.00m), 1);
            cart.Clear();
            Assert.That(cart.Items, Is.Empty);
            Assert.That(cart.TotalPrice, Is.EqualTo(0));
            Assert.That(cart.IsEmpty(), Is.True);
        }

        [Test]
        public void ConvertToOrder_WithItems_CreatesOrderWithAllItems()
        {
            var customerId = Guid.NewGuid();
            var cart = new ShoppingCart(customerId);
            cart.AddItem(new Product("Item 1", 10.00m), 2);
            cart.AddItem(new Product("Item 2", 15.00m), 3);
            var order = cart.ConvertToOrder();
            Assert.That(order, Is.Not.Null);
            Assert.That(order.CustomerId, Is.EqualTo(customerId));
            Assert.That(order.Items, Has.Count.EqualTo(2));
            Assert.That(order.TotalPrice, Is.EqualTo(65.00m));
        }

        [Test]
        public void ConvertToOrder_WithEmptyCart_ThrowsInvalidOperationException()
        {
            var cart = new ShoppingCart(Guid.NewGuid());
            Assert.Throws<InvalidOperationException>(() => cart.ConvertToOrder());
        }
    }
}