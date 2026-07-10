using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Enums;
using MediatR;

namespace ElectroShop.Application.Features.CreditSales.Queries.GetCreditSales;

public record GetCreditSalesQuery(
    int Page = 1,
    int PageSize = 10,
    string? Search = null,
    int? Status = null,
    CreditSaleProductSource? ProductSourceType = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    DateTime? DueFromDate = null,
    DateTime? DueToDate = null) : IRequest<PagedResult<CreditSaleListItemDto>>;
