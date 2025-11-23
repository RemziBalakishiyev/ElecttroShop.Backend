using ElectroShop.Application.DTOs;
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

    /// <summary>
    /// Dashboard üçün sifariş statistikaları
    /// </summary>
    Task<OrderStatisticsDto> GetOrderStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Son sifarişlər (Dashboard üçün)
    /// </summary>
    Task<List<Order>> GetRecentOrdersAsync(int count, CancellationToken cancellationToken = default);

    /// <summary>
    /// Zaman üzrə gəlir statistikaları (Chart üçün)
    /// </summary>
    Task<List<RevenueChartDataDto>> GetRevenueByDateAsync(
        string period,
        int periodCount,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Zaman üzrə sifariş sayı (Chart üçün)
    /// </summary>
    Task<List<OrderCountChartDataDto>> GetOrderCountByDateAsync(
        string period,
        int periodCount,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Status üzrə sifariş paylanması (Chart üçün)
    /// </summary>
    Task<List<OrderStatusChartDataDto>> GetOrdersByStatusAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Top məhsullar (Chart üçün)
    /// </summary>
    Task<List<TopProductChartDataDto>> GetTopProductsAsync(int count, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kateqoriya üzrə satışlar (Chart üçün)
    /// </summary>
    Task<List<CategorySalesChartDataDto>> GetSalesByCategoryAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Sifariş statistikaları
/// </summary>
public record OrderStatisticsDto
{
    public int TotalOrders { get; init; }
    public int OrdersThisMonth { get; init; }
    public decimal TotalRevenue { get; init; }
    public string RevenueCurrency { get; init; } = "AZN";
    public decimal RevenueThisMonth { get; init; }
    public int PendingOrders { get; init; }
    public int ProcessingOrders { get; init; }
    public int DeliveredOrders { get; init; }
}

