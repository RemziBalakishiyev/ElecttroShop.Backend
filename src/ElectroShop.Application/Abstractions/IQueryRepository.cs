using ElectroShop.Domain.Primitives;
using System.Linq.Expressions;

namespace ElectroShop.Application.Abstractions;

/// <summary>
/// Query (Read) operations üçün Generic Repository Interface
/// </summary>
public interface IQueryRepository<TEntity> where TEntity : BaseCommonEntity
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TEntity> GetByIdOrThrowAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// IQueryable qaytarır - Extension methods üçün
    /// Handler-də filtering və include aydın görünür
    /// </summary>
    IQueryable<TEntity> Query();
}




