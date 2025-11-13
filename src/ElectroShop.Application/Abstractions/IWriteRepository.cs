using ElectroShop.Domain.Primitives;

namespace ElectroShop.Application.Abstractions;

/// <summary>
/// Write (Command) operations için Generic Repository Interface
/// </summary>
public interface IWriteRepository<TEntity> where TEntity : BaseCommonEntity
{
    /// <summary>
    /// Yeni entity ekler
    /// </summary>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Birden fazla entity ekler
    /// </summary>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Entity'yi günceller
    /// </summary>
    void Update(TEntity entity);

    /// <summary>
    /// Birden fazla entity günceller
    /// </summary>
    void UpdateRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Entity'yi siler (hard delete)
    /// </summary>
    void Delete(TEntity entity);

    /// <summary>
    /// Birden fazla entity siler (hard delete)
    /// </summary>
    void DeleteRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// ID'ye göre entity siler
    /// </summary>
    Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Değişiklikleri veritabanına kaydeder
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}



