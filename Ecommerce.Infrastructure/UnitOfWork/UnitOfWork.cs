using Ecommerce.Infrastructure.Repositories;

namespace Ecommerce.Infrastructure.UnitOfWork;
public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private readonly ApplicationDbContext _context = context;

    public ICartRepository Carts { get; private set; } = new CartRepository(context);
    public IBaseRepository<Order, int> Orders { get; private set; } = new BaseRepository<Order, int>(context);
    public IBaseRepository<OrderItem, int> OrderItems { get; private set; } = new BaseRepository<OrderItem, int>(context);
    public IBaseRepository<CartItem, int> CartItems { get; private set; } = new BaseRepository<CartItem, int>(context);
    public IProductRepository Products { get; private set; } = new ProductRepository(context);
    public IBaseRepository<Category, int> Categories { get; private set; } = new BaseRepository<Category, int>(context);

    public void Dispose() => _context.Dispose();
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) 
        => await _context.SaveChangesAsync(cancellationToken);
}
