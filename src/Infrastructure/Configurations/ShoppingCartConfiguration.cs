using CustomerOrderManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerOrderManagement.Infrastructure.Configurations
{
    public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
    {
        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.CreatedOnUtc).IsRequired();
            builder.Property(e => e.UpdatedOnUtc).IsRequired();
            builder.Property(e => e.TotalPrice).HasPrecision(18, 2);


            builder.HasMany(sc => sc.Items)
                .WithOne(ci => ci.ShoppingCart)
                .HasForeignKey(ci => ci.ShoppingCartId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.CustomerId).IsUnique();
        }
    }
}