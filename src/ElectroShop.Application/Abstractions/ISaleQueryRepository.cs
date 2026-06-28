using ElectroShop.Domain.Entities;
using ElectroShop.Domain.Enums;

namespace ElectroShop.Application.Abstractions;

public interface ISaleQueryRepository : IQueryRepository<Sale>
{
    Task<(List<Sale> Sales, int TotalCount)> GetSalesPagedAsync(
        int page,
        int pageSize,
        string? search = null,
        Guid? categoryId = null,
        Guid? productId = null,
        SaleSource? saleSource = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        decimal? minProfit = null,
        decimal? maxProfit = null,
        CancellationToken cancellationToken = default);

    Task<Sale?> GetSaleByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
