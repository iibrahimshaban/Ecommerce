using Mapster;
using System.Linq.Expressions;

namespace Ecommerce.Infrastructure.Repositories;
public class BaseRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey> where TEntity : class
{
    protected readonly ApplicationDbContext _context;
    private readonly DbSet<TEntity> _entry;
    public BaseRepository(ApplicationDbContext context)
    {
        _context = context;
        _entry = _context.Set<TEntity>();
    }
    public async Task<TEntity?> GetByIdAsync(TKey id,CancellationToken cancellationToken = default)
    {
        return await _entry.FindAsync(id, cancellationToken);
    }
    public IQueryable<TEntity> Query(params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _entry.AsQueryable();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return query;
    }
    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _entry.ToListAsync(cancellationToken);
    }
    public async Task<TEntity> AddAsync(TEntity entity,CancellationToken cancellationToken = default)
    {
        await _entry.AddAsync(entity,cancellationToken);
        return entity;
    }
    public async Task<int> AddRangeAsync(IEnumerable<TEntity> entiries,CancellationToken cancellationToken = default)
    {
        foreach (var entity in entiries)
        {
            await _entry.AddAsync(entity, cancellationToken);
        }
            
        return entiries.Count();
    }
    public void Update(TEntity entity)
    {
        _entry.Update(entity);
    }
    public void Attach(TEntity entity)
    {
        _entry.Attach(entity);
    }
    public void Remove(TEntity entity)
    {
        _entry.Remove(entity);
    }
    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        _entry.RemoveRange(entities);
    }

    public async Task<TEntity?> Find(Expression<Func<TEntity, bool>> match,
         CancellationToken cancellationToken = default ,
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _entry;

        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return await query.FirstOrDefaultAsync(match);
    }
    public async Task<TEntity?> FindWithIncludesAsync(
       Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default ,
       params string[] includes )
    {
        IQueryable<TEntity> query = _entry.AsNoTracking();

        foreach (var include in includes)
        {
            query = query.Include(include); 
        }

        return await query.FirstOrDefaultAsync(predicate);
    }
    public async Task<IEnumerable<TEntity>> FindAllWithIncludesAsync(
       Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default ,
       params string[] includes )
    {
        IQueryable<TEntity> query = _entry.AsNoTracking();

        foreach (var include in includes)
        {
            query = query.Include(include); 
        }

        return await query.Where(predicate).ToListAsync(cancellationToken);
    }
    public async Task<IEnumerable<TEntity>> FindAll(Expression<Func<TEntity, bool>> match,
         CancellationToken cancellationToken = default ,
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _entry.AsNoTracking();
        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return await query.Where(match).ToListAsync();
    }
    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> match, CancellationToken cancellationToken = default)
    {
        return await _entry.AnyAsync(match,cancellationToken);
    }
}

