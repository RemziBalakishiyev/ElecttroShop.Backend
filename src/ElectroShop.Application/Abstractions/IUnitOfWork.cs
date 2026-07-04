namespace ElectroShop.Application.Abstractions;

/// <summary>
/// Unit of Work pattern - Birden fazla repository operasyonunu tek bir transaction içinde yönetir
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Tüm değişiklikleri veritabanına kaydeder
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Transaction başlatır
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Transaction'ı commit eder
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Transaction'ı rollback eder
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Entity-ni bazadan yenidən yükləyir (Concurrency Retry üçün)
    /// </summary>
    Task ReloadAsync(object entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Product aggregate child entity-lərinin EF state-ini SaveChanges-dən əvvəl düzəldir
    /// </summary>
    Task PrepareProductAggregateForSaveAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sale expense child entity-lərinin EF state-ini SaveChanges-dən əvvəl düzəldir
    /// </summary>
    Task PrepareSaleForSaveAsync(Guid saleId, CancellationToken cancellationToken = default);
}







