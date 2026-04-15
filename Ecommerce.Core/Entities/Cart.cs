using Ecommerce.Core.Const;

namespace Ecommerce.Core.Entities;
public sealed class Cart
{
    public int Id { get; set; }
    public CartStatus Status { get; set; } = CartStatus.Active;
    public double TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ICollection<CartItem> CartItems { get; set; } = [];
}
