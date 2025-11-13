using ElectroShop.Application.Abstractions;
using ElectroShop.Domain.Primitives;
using ElectroShop.Persistence.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ElectroShop.Persistence.Repositories;

public class WriteRepository<TEntity> : IWriteRepository<TEntity> where TEntity : BaseCommonEntity
{
    protected readonly ElectroShopDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;
    protected readonly IPublisher _publisher;

    public WriteRepository(ElectroShopDbContext context, IPublisher publisher)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
        _publisher = publisher;
    }

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
}

