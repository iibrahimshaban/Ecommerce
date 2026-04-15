namespace Ecommerce.Core.Entities;
public sealed class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Price { get; set; }
    public int Stock { get; set; }
    public int CategoryId { get; set; }
    public DateOnly? PublishedAt { get; set; }
    public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? RevokedAt { get; set; }
    public bool IsPublished { get; set; } = false;
    public Category Category { get; set; } = default!;
    public ICollection<CartItem> CartItems { get; set; } = [];
    public ICollection<OrderItem> OrderItems { get; set; } = [];
}
