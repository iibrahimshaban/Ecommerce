
namespace Ecommerce.Infrastructure.EntitiesConfigurations;
internal class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {

        builder
            .Property(ci => ci.Quantity)
            .IsRequired();

        builder.HasQueryFilter(i => i.Product != null && i.Product.IsPublished && i.Product.Stock >= i.Quantity);
    }
}
