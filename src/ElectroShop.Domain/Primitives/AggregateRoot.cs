namespace ElectroShop.Domain.Primitives;

/// <summary>
/// Aggregate Root - DDD'de bir aggregate'in kök entity'si
/// Sadece Aggregate Root'lar repository'den erişilebilir
/// RowVersion - Optimistic Concurrency Control üçün (PostgreSQL xmin → EF Core IsRowVersion)
/// </summary>
public abstract class AggregateRoot : BaseCommonEntity
{
    /// <summary>
    /// PostgreSQL xmin system column-una map olunur.
    /// DB-dən oxunur, client-ə göndərilir, update zamanı müqayisə edilir.
    /// </summary>
    public uint RowVersion { get; set; }
}
