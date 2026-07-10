using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.CreditSales.Queries.GetCreditSalesSummary;

public record GetCreditSalesSummaryQuery(
    int? Month = null,
    int? Year = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null) : IRequest<Result<CreditSaleSummaryDto>>;
