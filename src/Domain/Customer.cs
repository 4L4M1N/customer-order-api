using Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomerOrderManagement.Domain
{
    public class Customer : BaseEntity, ISoftDeletedEntity
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Address { get; private set; }
        public string PostalCode { get; private set; }
        protected Customer() { }
        private readonly List<Order> _orders = new();
        public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();
        public Customer(string firstName, string lastName, string address, string postalCode)
        {
            UpdateDetails(firstName, lastName, address, postalCode);
        }
        public void UpdateDetails(string firstName, string lastName, string address, string postalCode)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name is required", nameof(firstName));

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name is required", nameof(lastName));

            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Address is required", nameof(address));

            if (string.IsNullOrWhiteSpace(postalCode))
                throw new ArgumentException("Postal code is required", nameof(postalCode));

            FirstName = firstName;
            LastName = lastName;
            Address = address;
            PostalCode = postalCode;
        }
        public Order CreateOrder()
        {
            var order = new Order(Id);
            _orders.Add(order);
            return order;
        }

        public void RemoveOrder(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            _orders.Remove(order);
        }
        public IEnumerable<Order> GetOrdersByDate(DateTime? startDateUtc = null, DateTime? endDateUtc = null)
        {
            var query = _orders.AsQueryable();

            if (startDateUtc.HasValue)
                query = query.Where(o => o.CreatedOnUtc >= startDateUtc.Value);

            if (endDateUtc.HasValue)
                query = query.Where(o => o.CreatedOnUtc <= endDateUtc.Value);

            return query.OrderBy(o => o.CreatedOnUtc);
        }
        public ShoppingCart ShoppingCart { get; private set; }

        public bool IsDeleted { get; private set; }

        public DateTime? DeletedOnUtc { get; private set; }

        public void SoftDelete()
        {
            if (IsDeleted) return;

            IsDeleted = true;
            DeletedOnUtc = DateTime.UtcNow;
        }
    }
}