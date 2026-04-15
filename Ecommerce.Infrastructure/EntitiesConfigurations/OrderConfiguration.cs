

namespace Ecommerce.Infrastructure.EntitiesConfigurations;
internal class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder
            .Property(x => x.TotalAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder
            .Property(x => x.UserId).IsRequired();
    }
}
