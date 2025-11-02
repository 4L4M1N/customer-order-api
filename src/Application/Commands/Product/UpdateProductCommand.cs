using System;

namespace Application.Commands.Product
{
    public class UpdateProductCommand
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

    }
}