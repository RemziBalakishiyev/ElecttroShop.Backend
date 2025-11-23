using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using ElectroShop.Domain.Enums;

namespace ElectroShop.Application.Abstractions;

/// <summary>
/// Discount-specific query repository
/// </summary>
public interface IDiscountQueryRepository : IQueryRepository<Discount>
{
    /// <summary>
    /// Navigation properties ilə discount-ları səhifələnmiş şəkildə əldə edir
    /// </summary>
    Task<(List<Discount> Discounts, int TotalCount)> GetDiscountsPagedAsync(
        int page,
        int pageSize,
        DiscountType? type = null,
        bool? isActive = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Navigation properties ilə discount-u ID-yə görə əldə edir
    /// </summary>
    Task<Discount?> GetDiscountWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}

