

namespace Ecommerce.Core.Entities;
public sealed class OrderItem
{
    public int Id { get; set; }
    public double PriceAtPurchase { get; set; }
    public int Quantity { get; set; }
    public int ProductId { get; set; }
    public int OrderId { get; set; }
    public Product Product { get; set; } = default!;
    public Order Order { get; set; } = new Order();
}
