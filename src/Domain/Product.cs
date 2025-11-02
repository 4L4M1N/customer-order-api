using Domain;
using System;

namespace CustomerOrderManagement.Domain
{
    public class Product : BaseEntity, ISoftDeletedEntity
    {
        public string Name { get; private set; }

        public decimal Price { get; private set; }

        public bool IsDeleted { get; private set; }
        public DateTime? DeletedOnUtc { get; private set; }

        protected Product() { }

        public Product(string name, decimal price)
        {
            UpdateDetails(name, price);
        }

        public void UpdateDetails(string name, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name is required", nameof(name));

            if (price < 0)
                throw new ArgumentException("Price cannot be negative", nameof(price));

            Name = name;
            Price = price;
        }

        public void SoftDelete()
        {
            if (IsDeleted)
                return;

            IsDeleted = true;
            DeletedOnUtc = DateTime.UtcNow;
        }
    }
}