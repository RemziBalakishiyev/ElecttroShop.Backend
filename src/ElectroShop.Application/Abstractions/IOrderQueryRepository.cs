using ElectroShop.Domain.Entities;

namespace ElectroShop.Application.Abstractions;

/// <summary>
/// Order-specific query repository
/// </summary>
public interface IOrderQueryRepository : IQueryRepository<Order>
{
    /// <summary>
    /// Sifarişi ID-yə görə detal ilə tapır
    /// </summary>
    Task<Order?> GetOrderWithDetailsAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Müştəriyə görə səhifələnmiş sifariş siyahısı
    /// </summary>
    Task<(List<Order> Orders, int TotalCount)> GetOrdersByCustomerPagedAsync(
        Guid customerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}

