using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Filtering;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using ElectroShop.Domain.Entities;
using ElectroShop.Domain.Enums;
using ElectroShop.Persistence.Contexts;
using ElectroShop.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ElectroShop.Persistence.Repositories;

public class OrderQueryRepository : QueryRepository<Order>, IOrderQueryRepository
{
    private readonly IImageUrlResolver _imageUrlResolver;

    public OrderQueryRepository(ElectroShopDbContext context, IImageUrlResolver imageUrlResolver) : base(context)
    {
        _imageUrlResolver = imageUrlResolver;
    }

    public async Task<Order?> GetOrderWithDetailsAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Customer)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
    }

    public async Task<(List<Order> Orders, int TotalCount)> GetOrdersByCustomerPagedAsync(
        Guid customerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .AsNoTracking();

        var predicate = PredicateBuilder.True<Order>()
            .And(o => o.CustomerId == customerId);

        return await QueryHelper.ExecutePagedAsync(
            query.Where(predicate),
            page,
            pageSize,
            o => o.CreatedAtUtc,
            descending: true,
            cancellationToken);
    }

    public async Task<OrderStatisticsDto> GetOrderStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var totalOrders = await _dbSet.CountAsync(cancellationToken);

        var ordersThisMonth = await _dbSet
            .CountAsync(o => o.CreatedAtUtc >= startOfMonth, cancellationToken);

        // Ödənilmiş sifarişlərin ümumi gəliri
        var paidOrders = await _dbSet
            .Where(o => o.Status == OrderStatus.Paid || 
                       o.Status == OrderStatus.Processing || 
                       o.Status == OrderStatus.Shipped || 
                       o.Status == OrderStatus.Delivered)
            .ToListAsync(cancellationToken);

        var totalRevenue = paidOrders.Sum(o => o.Total.Amount);

        var paidOrdersThisMonth = await _dbSet
            .Where(o => (o.Status == OrderStatus.Paid || 
                        o.Status == OrderStatus.Processing || 
                        o.Status == OrderStatus.Shipped || 
                        o.Status == OrderStatus.Delivered) &&
                       o.CreatedAtUtc >= startOfMonth)
            .ToListAsync(cancellationToken);

        var revenueThisMonth = paidOrdersThisMonth.Sum(o => o.Total.Amount);

        var pendingOrders = await _dbSet
            .CountAsync(o => o.Status == OrderStatus.Pending, cancellationToken);

        var processingOrders = await _dbSet
            .CountAsync(o => o.Status == OrderStatus.Processing, cancellationToken);

        var deliveredOrders = await _dbSet
            .CountAsync(o => o.Status == OrderStatus.Delivered, cancellationToken);

        return new OrderStatisticsDto
        {
            TotalOrders = totalOrders,
            OrdersThisMonth = ordersThisMonth,
            TotalRevenue = totalRevenue,
            RevenueCurrency = "AZN",
            RevenueThisMonth = revenueThisMonth,
            PendingOrders = pendingOrders,
            ProcessingOrders = processingOrders,
            DeliveredOrders = deliveredOrders
        };
    }

    public async Task<List<Order>> GetRecentOrdersAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAtUtc)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<RevenueChartDataDto>> GetRevenueByDateAsync(
        string period,
        int periodCount,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var startDate = period.ToLower() switch
        {
            "daily" => now.AddDays(-periodCount),
            "weekly" => now.AddDays(-periodCount * 7),
            "monthly" => now.AddMonths(-periodCount),
            _ => now.AddMonths(-periodCount)
        };

        var paidOrders = await _dbSet
            .Where(o => o.CreatedAtUtc >= startDate &&
                       (o.Status == OrderStatus.Paid ||
                        o.Status == OrderStatus.Processing ||
                        o.Status == OrderStatus.Shipped ||
                        o.Status == OrderStatus.Delivered))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var grouped = period.ToLower() switch
        {
            "daily" => paidOrders
                .GroupBy(o => o.CreatedAtUtc.Date)
                .Select(g => new RevenueChartDataDto
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Revenue = g.Sum(o => o.Total.Amount),
                    Currency = "AZN",
                    OrderCount = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList(),
            "weekly" => paidOrders
                .GroupBy(o => GetWeekStart(o.CreatedAtUtc))
                .Select(g => new RevenueChartDataDto
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Revenue = g.Sum(o => o.Total.Amount),
                    Currency = "AZN",
                    OrderCount = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList(),
            "monthly" => paidOrders
                .GroupBy(o => new DateTime(o.CreatedAtUtc.Year, o.CreatedAtUtc.Month, 1))
                .Select(g => new RevenueChartDataDto
                {
                    Date = g.Key.ToString("yyyy-MM"),
                    Revenue = g.Sum(o => o.Total.Amount),
                    Currency = "AZN",
                    OrderCount = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList(),
            _ => new List<RevenueChartDataDto>()
        };

        return grouped;
    }

    public async Task<List<OrderCountChartDataDto>> GetOrderCountByDateAsync(
        string period,
        int periodCount,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var startDate = period.ToLower() switch
        {
            "daily" => now.AddDays(-periodCount),
            "weekly" => now.AddDays(-periodCount * 7),
            "monthly" => now.AddMonths(-periodCount),
            _ => now.AddMonths(-periodCount)
        };

        var orders = await _dbSet
            .Where(o => o.CreatedAtUtc >= startDate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var grouped = period.ToLower() switch
        {
            "daily" => orders
                .GroupBy(o => o.CreatedAtUtc.Date)
                .Select(g => new OrderCountChartDataDto
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList(),
            "weekly" => orders
                .GroupBy(o => GetWeekStart(o.CreatedAtUtc))
                .Select(g => new OrderCountChartDataDto
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList(),
            "monthly" => orders
                .GroupBy(o => new DateTime(o.CreatedAtUtc.Year, o.CreatedAtUtc.Month, 1))
                .Select(g => new OrderCountChartDataDto
                {
                    Date = g.Key.ToString("yyyy-MM"),
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList(),
            _ => new List<OrderCountChartDataDto>()
        };

        return grouped;
    }

    public async Task<List<OrderStatusChartDataDto>> GetOrdersByStatusAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _dbSet
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return orders
            .GroupBy(o => o.Status.ToString())
            .Select(g => new OrderStatusChartDataDto
            {
                Status = g.Key,
                Count = g.Count(),
                TotalAmount = g.Sum(o => o.Total.Amount),
                Currency = "AZN"
            })
            .OrderByDescending(x => x.Count)
            .ToList();
    }

    public async Task<List<TopProductChartDataDto>> GetTopProductsAsync(int count, CancellationToken cancellationToken = default)
    {
        var paidOrderIds = await _dbSet
            .Where(o => o.Status == OrderStatus.Paid ||
                       o.Status == OrderStatus.Processing ||
                       o.Status == OrderStatus.Shipped ||
                       o.Status == OrderStatus.Delivered)
            .Select(o => o.Id)
            .ToListAsync(cancellationToken);

        var orderItems = await _context.Set<OrderItem>()
            .Include(oi => oi.Product)
                .ThenInclude(p => p.Category)
            .Include(oi => oi.Product)
                .ThenInclude(p => p.ProductImages)
            .Where(oi => paidOrderIds.Contains(oi.OrderId))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var grouped = orderItems
            .GroupBy(oi => oi.ProductId)
            .Select(g =>
            {
                var product = g.First().Product;
                var primaryImage = product.ProductImages.FirstOrDefault(pi => pi.IsPrimary);
                var imageId = primaryImage?.ImageId
                    ?? product.ProductImages.OrderBy(pi => pi.DisplayOrder).FirstOrDefault()?.ImageId;
                var imageUrl = imageId.HasValue
                    ? _imageUrlResolver.BuildImageUrl(imageId.Value)
                    : null;

                return new TopProductChartDataDto
                {
                    ProductId = g.Key,
                    ProductName = product.Name,
                    ImageUrl = imageUrl,
                    TotalQuantitySold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.LineTotal.Amount),
                    Currency = "AZN",
                    OrderCount = g.Select(oi => oi.OrderId).Distinct().Count()
                };
            })
            .OrderByDescending(x => x.TotalQuantitySold)
            .Take(count)
            .ToList();

        return grouped;
    }

    public async Task<List<CategorySalesChartDataDto>> GetSalesByCategoryAsync(CancellationToken cancellationToken = default)
    {
        var paidOrderIds = await _dbSet
            .Where(o => o.Status == OrderStatus.Paid ||
                       o.Status == OrderStatus.Processing ||
                       o.Status == OrderStatus.Shipped ||
                       o.Status == OrderStatus.Delivered)
            .Select(o => o.Id)
            .ToListAsync(cancellationToken);

        var orderItems = await _context.Set<OrderItem>()
            .Include(oi => oi.Product)
                .ThenInclude(p => p.Category)
            .Where(oi => paidOrderIds.Contains(oi.OrderId))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var grouped = orderItems
            .GroupBy(oi => oi.Product.CategoryId)
            .Select(g => new CategorySalesChartDataDto
            {
                CategoryId = g.Key,
                CategoryName = g.First().Product.Category?.Name ?? "Unknown",
                TotalSales = g.Sum(oi => oi.LineTotal.Amount),
                Currency = "AZN",
                OrderCount = g.Select(oi => oi.OrderId).Distinct().Count(),
                ProductCount = g.Select(oi => oi.ProductId).Distinct().Count()
            })
            .OrderByDescending(x => x.TotalSales)
            .ToList();

        return grouped;
    }

    private static DateTime GetWeekStart(DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-1 * diff).Date;
    }
}

