using CustomerOrderManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerOrderManagement.Infrastructure.Configurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Quantity).IsRequired();
            builder.Property(e => e.UnitPrice).HasPrecision(18, 2);
            builder.Ignore(e => e.ItemTotal);

            builder.HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ci => ci.ShoppingCart)
                .WithMany(sc => sc.Items)
                .HasForeignKey(ci => ci.ShoppingCartId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}