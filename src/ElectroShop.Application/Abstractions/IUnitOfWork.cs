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
}







