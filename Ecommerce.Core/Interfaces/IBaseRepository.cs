using System.Linq.Expressions;

namespace Ecommerce.Core.Interfaces;
public interface IBaseRepository<TEntity, Tkey> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(Tkey id,CancellationToken cancellationToken = default);
    IQueryable<TEntity> Query(params Expression<Func<TEntity, object>>[] includes);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken =default);
    Task<TEntity> AddAsync(TEntity entity , CancellationToken cancellationToken = default);
    Task<int> AddRangeAsync(IEnumerable<TEntity> entiries, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Attach(TEntity entity);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);

    Task<TEntity?> Find(Expression<Func<TEntity, bool>> match, CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes);
    Task<TEntity?> FindWithIncludesAsync(Expression<Func<TEntity, bool>> predicate,CancellationToken cancellationToken = default, 
        params string[] includes);
    Task<IEnumerable<TEntity>> FindAllWithIncludesAsync(
       Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default,
       params string[] includes);
    Task<IEnumerable<TEntity>> FindAll(Expression<Func<TEntity, bool>> match, CancellationToken cancellationToken = default, 
        params Expression<Func<TEntity, object>>[] includes);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> match, CancellationToken cancellationToken = default);
}
