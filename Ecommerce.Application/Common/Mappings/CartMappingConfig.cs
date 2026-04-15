using Ecommerce.Application.Contracts.Cart;

namespace Ecommerce.Application.Common.Mappings;
public class CartMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Cart, CartResponse>()
              .Map(dest => dest.CartId, src => src.Id)
            .Map(dest => dest.Items, src => src.CartItems.Adapt<IEnumerable<ItemResponse>>())
            .Map(dest => dest.TotalAmount, src => src.CartItems.Sum(i => i.Quantity * i.Product.Price));

        config.NewConfig<CartItem, ItemResponse>()
            .Map(dest => dest.ProductName, src => src.Product.Name)
            .Map(dest => dest.Description, src => src.Product.Description)
            .Map(dest => dest.Price, src => src.Product.Price)
            .Map(dest => dest.Total, src => src.Quantity * src.Product.Price);
    }
}
