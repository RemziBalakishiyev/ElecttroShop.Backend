namespace ElectroShop.Domain.Primitives;

/// <summary>
/// Aggregate Root - DDD'de bir aggregate'in kök entity'si
/// Sadece Aggregate Root'lar repository'den erişilebilir
/// </summary>
public abstract class AggregateRoot : BaseCommonEntity
{
}
