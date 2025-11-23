namespace ElectroShop.Application.DTOs;

/// <summary>
/// Dashboard statistikaları üçün DTO
/// </summary>
public record DashboardDto
{
    /// <summary>
    /// Ümumi statistikalar
    /// </summary>
    public DashboardStatisticsDto Statistics { get; init; } = default!;

    /// <summary>
    /// Son məhsullar (5 ədəd)
    /// </summary>
    public List<ProductListDto> RecentProducts { get; init; } = new();

    /// <summary>
    /// Son sifarişlər (5 ədəd)
    /// </summary>
    public List<OrderSummaryDto> RecentOrders { get; init; } = new();
}

/// <summary>
/// Dashboard statistikaları
/// </summary>
public record DashboardStatisticsDto
{
    /// <summary>
    /// Ümumi məhsul sayı
    /// </summary>
    public int TotalProducts { get; init; }

    /// <summary>
    /// Aktiv məhsul sayı
    /// </summary>
    public int ActiveProducts { get; init; }

    /// <summary>
    /// Ümumi sifariş sayı
    /// </summary>
    public int TotalOrders { get; init; }

    /// <summary>
    /// Bu ay sifariş sayı
    /// </summary>
    public int OrdersThisMonth { get; init; }

    /// <summary>
    /// Ümumi müştəri sayı
    /// </summary>
    public int TotalCustomers { get; init; }

    /// <summary>
    /// Ümumi kateqoriya sayı
    /// </summary>
    public int TotalCategories { get; init; }

    /// <summary>
    /// Ümumi brend sayı
    /// </summary>
    public int TotalBrands { get; init; }

    /// <summary>
    /// Ümumi gəlir (ödənilmiş sifarişlərin cəmi)
    /// </summary>
    public decimal TotalRevenue { get; init; }

    /// <summary>
    /// Gəlir valyutası
    /// </summary>
    public string RevenueCurrency { get; init; } = "AZN";

    /// <summary>
    /// Bu ay gəlir
    /// </summary>
    public decimal RevenueThisMonth { get; init; }

    /// <summary>
    /// Gözləmədə olan sifarişlər
    /// </summary>
    public int PendingOrders { get; init; }

    /// <summary>
    /// Hazırlanan sifarişlər
    /// </summary>
    public int ProcessingOrders { get; init; }

    /// <summary>
    /// Çatdırılan sifarişlər
    /// </summary>
    public int DeliveredOrders { get; init; }
}

/// <summary>
/// Sifariş xülasəsi (Dashboard üçün)
/// </summary>
public record OrderSummaryDto
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerEmail { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal Total { get; init; }
    public string Currency { get; init; } = "AZN";
    public int ItemCount { get; init; }
    public DateTime CreatedAt { get; init; }
}


