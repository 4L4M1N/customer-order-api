using Domain;
using System;

namespace CustomerOrderManagement.Domain
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; private set; }


        public Guid ProductId { get; private set; }


        public int Quantity { get; private set; }


        public decimal UnitPrice { get; private set; }

        public decimal ItemTotal => Quantity * UnitPrice;

        public Product Product { get; private set; }

        public Order Order { get; private set; }

        protected OrderItem() { }
        public OrderItem(Guid orderId, Guid productId, int quantity, decimal unitPrice)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

            OrderId = orderId;
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
    }
}