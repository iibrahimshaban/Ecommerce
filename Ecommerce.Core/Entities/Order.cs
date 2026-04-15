using Ecommerce.Core.Const;

namespace Ecommerce.Core.Entities;
public sealed class Order
{
    public int Id { get; set; }
    public double TotalAmount { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string UserId { get; set; } = string.Empty;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public ICollection<OrderItem> OrderItems { get; set; } = [];
}
