using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Sales.Queries.ExportMonthlySalesPdf;

public record ExportMonthlySalesPdfQuery(int Year, int Month)
    : IRequest<Result<SalesExportFileDto>>;
