using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Enums;
using MediatR;

namespace ElectroShop.Application.Features.Sales.Queries.GetSales;

public record GetSalesQuery(
    int Page = 1,
    int PageSize = 10,
    string? Search = null,
    Guid? CategoryId = null,
    Guid? ProductId = null,
    SaleSource? SaleSource = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    decimal? MinProfit = null,
    decimal? MaxProfit = null,
    decimal? MinExpense = null,
    decimal? MaxExpense = null) : IRequest<PagedResult<SaleListItemDto>>;
