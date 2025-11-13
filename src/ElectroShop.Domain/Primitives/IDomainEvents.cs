using MediatR;

namespace ElectroShop.Domain.Primitives;

/// <summary>
/// Domain Event marker interface
/// MediatR.INotification'dan türer çünkü event'leri MediatR ile yayınlıyoruz
/// </summary>
public interface IDomainEvent : INotification
{
    DateTime OccurredOnUtc { get; }
}
