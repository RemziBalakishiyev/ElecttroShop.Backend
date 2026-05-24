using ElectroShop.Application.Abstractions;
using ElectroShop.Domain.Entities;
using ElectroShop.Domain.Exceptions;
using ElectroShop.Domain.Primitives;
using ElectroShop.Persistence.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ElectroShop.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ElectroShopDbContext _context;
    private readonly IPublisher _publisher;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(ElectroShopDbContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = _context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();

        try
        {
            var result = await _context.SaveChangesAsync(cancellationToken);

            if (domainEvents.Any())
            {
                foreach (var domainEvent in domainEvents)
                {
                    await _publisher.Publish(domainEvent, cancellationToken);
                }

                _context.ChangeTracker
                    .Entries<BaseEntity>()
                    .Where(e => e.Entity.DomainEvents.Any())
                    .ToList()
                    .ForEach(e => e.Entity.ClearDomainEvents());
            }

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException(
                "Məlumat başqa istifadəçi tərəfindən dəyişdirilib. Yenidən yükləyin.",
                ex);
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);

            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task ReloadAsync(object entity, CancellationToken cancellationToken = default)
    {
        await _context.Entry(entity).ReloadAsync(cancellationToken);
    }

    public async Task PrepareProductAggregateForSaveAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var imageEntries = _context.ChangeTracker.Entries<ProductImage>()
            .Where(e => e.Entity.ProductId == productId)
            .ToList();

        if (imageEntries.Count > 0)
        {
            var imageIds = imageEntries.Select(e => e.Entity.Id).ToList();
            var existingImageIds = await _context.ProductImages
                .AsNoTracking()
                .Where(pi => imageIds.Contains(pi.Id))
                .Select(pi => pi.Id)
                .ToListAsync(cancellationToken);

            FixChildEntityStates(imageEntries, existingImageIds.ToHashSet(), e => e.Entity.Id);
        }

        var variantEntries = _context.ChangeTracker.Entries<ProductVariant>()
            .Where(e => e.Entity.ProductId == productId)
            .ToList();

        if (variantEntries.Count > 0)
        {
            var variantIds = variantEntries.Select(e => e.Entity.Id).ToList();
            var existingVariantIds = await _context.ProductVariants
                .AsNoTracking()
                .Where(pv => variantIds.Contains(pv.Id))
                .Select(pv => pv.Id)
                .ToListAsync(cancellationToken);

            FixChildEntityStates(variantEntries, existingVariantIds.ToHashSet(), e => e.Entity.Id);
        }
    }

    private static void FixChildEntityStates<TEntity>(
        List<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TEntity>> entries,
        HashSet<Guid> existingIds,
        Func<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TEntity>, Guid> getId)
        where TEntity : class
    {
        foreach (var entry in entries)
        {
            var exists = existingIds.Contains(getId(entry));

            if (entry.State == EntityState.Modified && !exists)
                entry.State = EntityState.Added;
            else if (entry.State == EntityState.Deleted && !exists)
                entry.State = EntityState.Detached;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
