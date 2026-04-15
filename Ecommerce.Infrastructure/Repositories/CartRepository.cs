using Mapster;
using System.Linq.Expressions;

namespace Ecommerce.Infrastructure.Repositories;
public class CartRepository(ApplicationDbContext context) : BaseRepository<Cart, int>(context), ICartRepository
{
    public async Task<TResult> ProjectToType<TResult>(Expression<Func<Cart, bool>> match, CancellationToken cancellationToken = default)
    {
        var cart = await _context.Carts
            .Where(match)
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)!
            .ProjectToType<TResult>()
            .FirstOrDefaultAsync(cancellationToken);

        return cart;
    }
    public async Task RemoveItemAsync(CartItem cartItem)
    {
        _context.CartItems.Remove(cartItem);
        await Task.CompletedTask;
    }
}
