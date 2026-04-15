namespace Ecommerce.Infrastructure.EntitiesConfigurations;
internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder
            .Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();
    }
}
