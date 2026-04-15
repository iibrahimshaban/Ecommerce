

using Ecommerce.Shared.Authorization;

namespace Ecommerce.Infrastructure.EntitiesConfigurations;
internal class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasOne(u => u.Cart)
               .WithOne()
               .HasForeignKey<Cart>(c => c.UserId);

        builder
            .HasMany(u => u.Orders)
            .WithOne()
            .HasForeignKey(o => o.UserId);

        builder
              .Property(x => x.FirstName)
              .HasMaxLength(150);
        builder
            .Property(x => x.LastName)
            .HasMaxLength(150);

        builder.OwnsMany(x => x.RefreshTokens)
            .ToTable("RefreshTokens")
            .WithOwner()
            .HasForeignKey("UserId");

        var appUser = new ApplicationUser
        {
            Id = DefaultUsers.AdminId,
            FirstName = "Ibrahim",
            LastName = "khaled",
            Email = DefaultUsers.AdminEmail,
            NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
            UserName = "iibrahim",
            NormalizedUserName = "IIBRAHIM",
            SecurityStamp = DefaultUsers.AdminSequrityStamp,
            ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
            EmailConfirmed = true,
            PasswordHash = DefaultUsers.AdminHashedPassword,
            Cart = null, 
            Orders = null
        };

        builder.HasData(appUser);
    }
}
