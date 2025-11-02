using Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomerOrderManagement.Domain
{
    public class ShoppingCart : BaseEntity
    {
        private readonly List<CartItem> _items = new();

        public Guid CustomerId { get; private set; }
        public DateTime CreatedOnUtc { get; private set; }

        public DateTime UpdatedOnUtc { get; private set; }

        public decimal TotalPrice { get; private set; }

        public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

        public Customer Customer { get; private set; }

        protected ShoppingCart() { }
        public ShoppingCart(Guid customerId)
        {
            CustomerId = customerId;
            CreatedOnUtc = DateTime.UtcNow;
            UpdatedOnUtc = DateTime.UtcNow;
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
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            else
            {
                var cartItem = new CartItem(product.Id, quantity, product.Price);
                cartItem.AttachToCart(Id);
                _items.Add(cartItem);
            }

            UpdateLastModified();
            RecalculateTotalPrice();
        }

        public void UpdateItemQuantity(Guid productId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            var item = _items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
                throw new InvalidOperationException("Item not found in cart");

            item.UpdateQuantity(quantity);
            UpdateLastModified();
            RecalculateTotalPrice();
        }

        public void RemoveItem(Guid productId)
        {
            var item = _items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                _items.Remove(item);
                UpdateLastModified();
                RecalculateTotalPrice();
            }
        }

        public void Clear()
        {
            _items.Clear();
            UpdateLastModified();
            RecalculateTotalPrice();
        }

        public bool IsEmpty() => !_items.Any();

        public Order ConvertToOrder()
        {
            if (IsEmpty())
                throw new InvalidOperationException("Cannot create order from empty cart");

            var order = new Order(CustomerId);

            foreach (var cartItem in _items)
            {
                order.AddItemDirectly(cartItem.ProductId, cartItem.Quantity, cartItem.UnitPrice);
            }

            return order;
        }

        private void RecalculateTotalPrice()
        {
            TotalPrice = _items.Sum(i => i.ItemTotal);
        }

        private void UpdateLastModified()
        {
            UpdatedOnUtc = DateTime.UtcNow;
        }
    }
    public class CartItem
    {
        public Guid Id { get; private set; }

        public Guid ShoppingCartId { get; private set; }

        public Guid ProductId { get; private set; }

        public int Quantity { get; private set; }

        public decimal UnitPrice { get; private set; }

        public decimal ItemTotal => Quantity * UnitPrice;

        public Product Product { get; private set; }

        public ShoppingCart ShoppingCart { get; private set; }

        protected CartItem() { }

        public CartItem(Guid productId, int quantity, decimal unitPrice)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

            Id = Guid.NewGuid();
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public void UpdateQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            Quantity = quantity;
        }

        internal void AttachToCart(Guid cartId)
        {
            ShoppingCartId = cartId;
        }
    }
}
