using Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomerOrderManagement.Domain
{
    public class Order : BaseEntity
    {
        public DateTime CreatedOnUtc { get; private set; }

        private readonly List<OrderItem> _items = new();
        public Guid CustomerId { get; private set; }

        public decimal TotalPrice { get; private set; }

        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        public Customer Customer { get; private set; }

        protected Order() { }
        public Order(Guid customerId)
        {
            CustomerId = customerId;
            CreatedOnUtc = DateTime.UtcNow;
            TotalPrice = 0;
        }

        public void AddItem(Product product, int quantity)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            var existingItem = _items.FirstOrDefault(i => i.ProductId == product.Id);

            if (existingItem != null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                var item = new OrderItem(Id, product.Id, quantity, product.Price);
                _items.Add(item);
            }

            RecalculateTotalPrice();
        }

        public void RemoveItem(Guid productId)
        {
            var item = _items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                _items.Remove(item);
                RecalculateTotalPrice();
            }
        }

        public void UpdateItemQuantity(Guid productId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            var item = _items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
                throw new InvalidOperationException("Item not found in order");

            item.UpdateQuantity(quantity);
            RecalculateTotalPrice();
        }

        private void RecalculateTotalPrice()
        {
            TotalPrice = _items.Sum(i => i.ItemTotal);
        }
        internal void AddItemDirectly(Guid productId, int quantity, decimal unitPrice)
        {
            var item = new OrderItem(Id, productId, quantity, unitPrice);
            _items.Add(item);
            RecalculateTotalPrice();
        }
    }
}