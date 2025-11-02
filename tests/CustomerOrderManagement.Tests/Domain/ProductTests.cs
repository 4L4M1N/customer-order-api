using CustomerOrderManagement.Domain;

namespace CustomerOrderManagement.Tests.Domain
{
    [TestFixture]
    public class ProductTests
    {
        [Test]
        public void Constructor_WithValidData_CreatesProduct()
        {
            var product = new Product("Test Product", 19.99m);
            Assert.That(product.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(product.Name, Is.EqualTo("Test Product"));
            Assert.That(product.Price, Is.EqualTo(19.99m));
        }

        [Test]
        public void Constructor_WithEmptyName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Product("", 10.00m));
        }

        [Test]
        public void Constructor_WithNegativePrice_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Product("Product", -5.00m));
        }

        [Test]
        public void UpdateDetails_WithValidData_UpdatesProduct()
        {
            var product = new Product("Old Name", 10.00m);
            product.UpdateDetails("New Name", 15.00m);
            Assert.That(product.Name, Is.EqualTo("New Name"));
            Assert.That(product.Price, Is.EqualTo(15.00m));
        }
    }
}