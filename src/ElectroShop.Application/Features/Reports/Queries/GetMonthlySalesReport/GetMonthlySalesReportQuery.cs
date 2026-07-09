using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Reports.Queries.GetMonthlySalesReport;

public record GetMonthlySalesReportQuery(int Year, int Month)
    : IRequest<Result<MonthlySalesReportDto>>;
