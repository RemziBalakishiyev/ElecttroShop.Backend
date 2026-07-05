using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Dashboard.Queries.GetDashboard;

/// <summary>
/// Dashboard məlumatlarını əldə etmək üçün Handler
/// </summary>
public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, Result<DashboardDto>>
{
    private readonly IProductQueryRepository _productRepository;
    private readonly IOrderQueryRepository _orderRepository;
    private readonly ICustomerQueryRepository _customerRepository;
    private readonly ICategoryQueryRepository _categoryRepository;
    private readonly IBrandQueryRepository _brandRepository;
    private readonly IImageUrlResolver _imageUrlResolver;

    public GetDashboardQueryHandler(
        IProductQueryRepository productRepository,
        IOrderQueryRepository orderRepository,
        ICustomerQueryRepository customerRepository,
        ICategoryQueryRepository categoryRepository,
        IBrandQueryRepository brandRepository,
        IImageUrlResolver imageUrlResolver)
    {
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _categoryRepository = categoryRepository;
        _brandRepository = brandRepository;
        _imageUrlResolver = imageUrlResolver;
    }

    public async Task<Result<DashboardDto>> Handle(
        GetDashboardQuery request,
        CancellationToken cancellationToken)
    {
        // DbContext thread-safe olmadığı üçün ardıcıl olaraq işlədirik
        var statistics = await GetStatisticsAsync(cancellationToken);
        var recentProducts = await GetRecentProductsAsync(cancellationToken);
        var recentOrders = await GetRecentOrdersAsync(cancellationToken);

        var dashboard = new DashboardDto
        {
            Statistics = statistics,
            RecentProducts = recentProducts,
            RecentOrders = recentOrders
        };

        return Result.Success(dashboard);
    }

    private async Task<DashboardStatisticsDto> GetStatisticsAsync(CancellationToken cancellationToken)
    {
        // DbContext thread-safe olmadığı üçün ardıcıl olaraq işlədirik
        var totalProducts = await _productRepository.CountAsync(cancellationToken: cancellationToken);
        var activeProducts = await _productRepository.CountAsync(p => p.IsActive, cancellationToken);
        var totalCustomers = await _customerRepository.CountAsync(cancellationToken: cancellationToken);
        var totalCategories = await _categoryRepository.CountAsync(cancellationToken: cancellationToken);
        var totalBrands = await _brandRepository.CountAsync(cancellationToken: cancellationToken);
        var orderStatistics = await _orderRepository.GetOrderStatisticsAsync(cancellationToken);

        return new DashboardStatisticsDto
        {
            TotalProducts = totalProducts,
            ActiveProducts = activeProducts,
            TotalOrders = orderStatistics.TotalOrders,
            OrdersThisMonth = orderStatistics.OrdersThisMonth,
            TotalCustomers = totalCustomers,
            TotalCategories = totalCategories,
            TotalBrands = totalBrands,
            TotalRevenue = orderStatistics.TotalRevenue,
            RevenueCurrency = orderStatistics.RevenueCurrency,
            RevenueThisMonth = orderStatistics.RevenueThisMonth,
            PendingOrders = orderStatistics.PendingOrders,
            ProcessingOrders = orderStatistics.ProcessingOrders,
            DeliveredOrders = orderStatistics.DeliveredOrders
        };
    }

    private async Task<List<ProductListDto>> GetRecentProductsAsync(CancellationToken cancellationToken)
    {
        var (products, _) = await _productRepository.GetProductsPagedAsync(
            page: 1,
            pageSize: 5,
            cancellationToken: cancellationToken);

        var productDtos = new List<ProductListDto>();
        foreach (var product in products)
        {
            // PrimaryImageUrl-i set et
            var primaryImage = product.ProductImages
                .OrderBy(pi => pi.IsPrimary ? 0 : 1)
                .ThenBy(pi => pi.DisplayOrder)
                .FirstOrDefault();
            
            string? primaryImageUrl = null;
            if (primaryImage != null)
            {
                primaryImageUrl = await _imageUrlResolver.ResolveProductImageUrlAsync(primaryImage, cancellationToken);
            }

            var productDto = product.Adapt<ProductListDto>();
            productDto = productDto with
            {
                PrimaryImageUrl = primaryImageUrl
            };
            productDtos.Add(productDto);
        }

        return productDtos;
    }

    private async Task<List<OrderSummaryDto>> GetRecentOrdersAsync(CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetRecentOrdersAsync(5, cancellationToken);

        return orders.Select(o => new OrderSummaryDto
        {
            Id = o.Id,
            CustomerId = o.CustomerId,
            CustomerName = o.Customer.FullName,
            CustomerEmail = o.Customer.Email,
            Status = o.Status.ToString(),
            Total = o.Total.Amount,
            Currency = o.Total.Currency,
            ItemCount = o.Items.Count,
            CreatedAt = o.CreatedAtUtc
        }).ToList();
    }
}

