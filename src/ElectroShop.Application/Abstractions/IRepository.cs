using ElectroShop.Domain.Primitives;

namespace ElectroShop.Application.Abstractions;

/// <summary>
/// Hem Write hem Query operasyonlarını bir arada sunan Repository Interface
/// Basit senaryolar için kullanılabilir
/// </summary>
public interface IRepository<TEntity> : IWriteRepository<TEntity>, IQueryRepository<TEntity>
    where TEntity : BaseCommonEntity
{
}



