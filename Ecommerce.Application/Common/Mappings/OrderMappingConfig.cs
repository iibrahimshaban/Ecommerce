using Ecommerce.Application.Contracts.Orders;
using Ecommerce.Core.Const;

namespace Ecommerce.Application.Common.Mappings;
public class OrderMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
       config.NewConfig<OrderItem,OrderItemResponse>()
            .Map(dest => dest.ProductName, src => src.Product.Name)
            .Map(dest => dest.UnitPrice, src => src.Product.Price)
            .Map(dest => dest.Quantity, src => src.Quantity)
            .Map(dest => dest.Subtotal, src => src.Quantity * src.Product.Price);

        config.NewConfig<Order, OrderResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.TotalPrice, src => src.TotalAmount)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.Status, src => src.Status.ToString())
            .Map(dest => dest.Items, src => src.OrderItems);

        config.NewConfig<Cart, Order>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.TotalAmount, src => src.TotalPrice)
            .Map(dest => dest.Status, _ => OrderStatus.Pending)
            .Map(dest => dest.CreatedAt, _ => DateTime.UtcNow)
            .Map(dest => dest.OrderItems, src => src.CartItems.Adapt<List<OrderItem>>())
            .Ignore(dest => dest.Id); 


        config.NewConfig<CartItem,OrderItem>()
            .Map(dest => dest.ProductId, src => src.ProductId)
            .Map(dest => dest.Quantity, src => src.Quantity)
            .Map(dest => dest.PriceAtPurchase, src => src.Product.Price)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Order)
            .Ignore(dest => dest.Product);
    }
}
