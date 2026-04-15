using Ecommerce.Core.Entities;

namespace Ecommerce.Core.Interfaces;
public interface IUnitOfWork : IDisposable
{
    ICartRepository Carts { get; }
    IBaseRepository<Order,int> Orders { get; }
    IBaseRepository<OrderItem,int> OrderItems { get; }
    IBaseRepository<CartItem,int> CartItems { get; }
    IProductRepository Products { get; }
    IBaseRepository<Category,int> Categories { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
