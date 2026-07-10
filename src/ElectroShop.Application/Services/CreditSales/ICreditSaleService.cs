using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;

namespace ElectroShop.Application.Services.CreditSales;

public interface ICreditSaleService
{
    Task<Result<CreditSaleDetailDto>> CreateAsync(
        CreateCreditSaleRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<CreditSaleDetailDto>> UpdateAsync(
        Guid id,
        UpdateCreditSaleRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> CancelAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<CreditSaleDetailDto>> MarkAsSoldAsync(
        Guid id,
        DateTime? soldDate,
        CancellationToken cancellationToken = default);
}
