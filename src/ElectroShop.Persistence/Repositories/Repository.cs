using ElectroShop.Application.Abstractions;
using ElectroShop.Domain.Primitives;
using ElectroShop.Persistence.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElectroShop.Persistence.Repositories;

/// <summary>
/// Hem Write hem Query operasyonlarını bir arada sunan Generic Repository
/// </summary>
public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseCommonEntity
{
    protected readonly ElectroShopDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;
    protected readonly IPublisher _publisher;

    public Repository(ElectroShopDbContext context, IPublisher publisher)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
        _publisher = publisher;
    }

    #region Write Operations

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public void DeleteRange(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public async Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Domain Events'leri topla
        var domainEvents = _context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();

        // Değişiklikleri kaydet
        var result = await _context.SaveChangesAsync(cancellationToken);

        // Domain Events'leri yayınla
        if (domainEvents.Any())
        {
            foreach (var domainEvent in domainEvents)
            {
                await _publisher.Publish(domainEvent, cancellationToken);
            }

            // Events'leri temizle
            _context.ChangeTracker
                .Entries<BaseEntity>()
                .Where(e => e.Entity.DomainEvents.Any())
                .ToList()
                .ForEach(e => e.Entity.ClearDomainEvents());
        }

        return result;
    }

    #endregion

    #region Query Operations

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<TEntity> GetByIdOrThrowAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        return entity ?? throw new KeyNotFoundException($"{typeof(TEntity).Name} with ID {id} not found.");
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

    public IQueryable<TEntity> AsQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public IQueryable<TEntity> AsNoTracking()
    {
        return _dbSet.AsNoTracking();
    }

    public IQueryable<TEntity> Query()
    {
        return _dbSet.AsNoTracking();
    }

    #endregion
}


