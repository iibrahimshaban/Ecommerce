
namespace Ecommerce.Infrastructure.EntitiesConfigurations;
internal class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder
            .Property(c => c.Status)
            .HasConversion<int>() 
            .IsRequired();

        builder
            .Property(c => c.TotalPrice)
            .HasColumnType("decimal(18,2)");
    }
}
