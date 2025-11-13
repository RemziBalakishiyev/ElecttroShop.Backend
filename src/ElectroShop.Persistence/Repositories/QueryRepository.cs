using ElectroShop.Application.Abstractions;
using ElectroShop.Domain.Primitives;
using ElectroShop.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElectroShop.Persistence.Repositories;

/// <summary>
/// Query Repository - Sadə və aydın
/// </summary>
public class QueryRepository<TEntity> : IQueryRepository<TEntity> where TEntity : BaseCommonEntity
{
    protected readonly ElectroShopDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public QueryRepository(ElectroShopDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<TEntity> GetByIdOrThrowAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        return entity ?? throw new KeyNotFoundException($"{typeof(TEntity).Name} tapılmadı: {id}");
    }

    public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<List<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<TEntity?> SingleOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        return predicate == null
            ? await _dbSet.CountAsync(cancellationToken)
            : await _dbSet.CountAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// IQueryable qaytarır - Extension methods ilə istifadə üçün
    /// AsNoTracking avtomatik tətbiq olunur
    /// </summary>
    public IQueryable<TEntity> Query()
    {
        return _dbSet.AsNoTracking();
    }
}




