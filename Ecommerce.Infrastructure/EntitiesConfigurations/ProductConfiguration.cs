
namespace Ecommerce.Infrastructure.EntitiesConfigurations;
internal class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder
            .Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();
        builder
            .Property(x => x.Description)
            .HasMaxLength(1000);
        builder
            .Property(x => x.Price)
            .IsRequired();
        builder
            .Property(x => x.Stock)
            .IsRequired();

    }
}
