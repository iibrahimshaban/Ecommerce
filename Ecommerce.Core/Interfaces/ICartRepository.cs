using Ecommerce.Core.Entities;
using System.Linq.Expressions;

namespace Ecommerce.Core.Interfaces;
public interface ICartRepository : IBaseRepository<Cart,int>
{
    Task<TResult> ProjectToType<TResult>(Expression<Func<Cart, bool>> match, CancellationToken cancellationToken = default);
    Task RemoveItemAsync(CartItem cartItem);
}
