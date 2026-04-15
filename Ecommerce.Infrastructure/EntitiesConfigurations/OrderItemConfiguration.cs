namespace Ecommerce.Infrastructure.EntitiesConfigurations;
internal class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder
            .Property(oi => oi.Quantity)
            .IsRequired();

        builder
            .Property(oi => oi.PriceAtPurchase)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
    }
}
