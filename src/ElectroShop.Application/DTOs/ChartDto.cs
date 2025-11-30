namespace ElectroShop.Application.DTOs;

/// <summary>
/// Chart məlumatları üçün DTO
/// </summary>
public record ChartDataDto
{
    /// <summary>
    /// Zaman üzrə gəlir statistikaları (günlük, həftəlik, aylıq)
    /// </summary>
    public List<RevenueChartDataDto> RevenueByDate { get; init; } = new();

    /// <summary>
    /// Zaman üzrə sifariş sayı
    /// </summary>
    public List<OrderCountChartDataDto> OrderCountByDate { get; init; } = new();

    /// <summary>
    /// Kateqoriya üzrə satışlar
    /// </summary>
    public List<CategorySalesChartDataDto> SalesByCategory { get; init; } = new();

    /// <summary>
    /// Status üzrə sifariş paylanması
    /// </summary>
    public List<OrderStatusChartDataDto> OrdersByStatus { get; init; } = new();

    /// <summary>
    /// Top 10 ən çox satılan məhsullar
    /// </summary>
    public List<TopProductChartDataDto> TopProducts { get; init; } = new();
}

/// <summary>
/// Zaman üzrə gəlir chart məlumatı
/// </summary>
public record RevenueChartDataDto
{
    public string Date { get; init; } = string.Empty; // Format: "YYYY-MM-DD" və ya "YYYY-MM"
    public decimal Revenue { get; init; }
    public string Currency { get; init; } = "AZN";
    public int OrderCount { get; init; }
}

/// <summary>
/// Zaman üzrə sifariş sayı chart məlumatı
/// </summary>
public record OrderCountChartDataDto
{
    public string Date { get; init; } = string.Empty; // Format: "YYYY-MM-DD" və ya "YYYY-MM"
    public int Count { get; init; }
}

/// <summary>
/// Kateqoriya üzrə satış chart məlumatı
/// </summary>
public record CategorySalesChartDataDto
{
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public decimal TotalSales { get; init; }
    public string Currency { get; init; } = "AZN";
    public int OrderCount { get; init; }
    public int ProductCount { get; init; }
}

/// <summary>
/// Status üzrə sifariş chart məlumatı
/// </summary>
public record OrderStatusChartDataDto
{
    public string Status { get; init; } = string.Empty;
    public int Count { get; init; }
    public decimal TotalAmount { get; init; }
    public string Currency { get; init; } = "AZN";
}

/// <summary>
/// Top məhsul chart məlumatı
/// </summary>
public record TopProductChartDataDto
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public int TotalQuantitySold { get; init; }
    public decimal TotalRevenue { get; init; }
    public string Currency { get; init; } = "AZN";
    public int OrderCount { get; init; }
}




