using CustomerOrderManagement.Domain;

namespace CustomerOrderManagement.Tests.Domain
{
    [TestFixture]
    public class CustomerTests
    {
        [Test]
        public void Constructor_WithValidData_CreatesCustomer()
        {
            var customer = new Customer("Test", "Customer", "address", "12345");
            Assert.That(customer.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(customer.FirstName, Is.EqualTo("Test"));
            Assert.That(customer.LastName, Is.EqualTo("Customer"));
            Assert.That(customer.Address, Is.EqualTo("address"));
            Assert.That(customer.PostalCode, Is.EqualTo("12345"));
        }

        [Test]
        public void Constructor_WithEmptyFirstName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Customer("", "Customer", "address", "12345"));
        }

        [Test]
        public void Constructor_WithEmptyLastName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Customer("Test", "", "address", "12345"));
        }

        [Test]
        public void UpdateDetails_WithValidData_UpdatesCustomer()
        {
            var customer = new Customer("Test", "Customer", "address", "12345");
            customer.UpdateDetails("Who", "Smith", "address", "67890");
            Assert.That(customer.FirstName, Is.EqualTo("Who"));
            Assert.That(customer.LastName, Is.EqualTo("Smith"));
            Assert.That(customer.Address, Is.EqualTo("address"));
            Assert.That(customer.PostalCode, Is.EqualTo("67890"));
        }

        [Test]
        public void CreateOrder_CreatesNewOrder()
        {
            var customer = new Customer("Test", "Customer", "address", "12345");
            var order = customer.CreateOrder();

            Assert.That(order, Is.Not.Null);
            Assert.That(order.CustomerId, Is.EqualTo(customer.Id));
            Assert.That(customer.Orders, Has.Count.EqualTo(1));
        }

        [Test]
        public void GetOrdersByDate_WithoutFilters_ReturnsAllOrders()
        {
            var customer = new Customer("Test", "Customer", "address", "12345");
            customer.CreateOrder();
            customer.CreateOrder();
            var orders = customer.GetOrdersByDate();
            Assert.That(orders.ToList(), Has.Count.EqualTo(2));
        }
    }
}