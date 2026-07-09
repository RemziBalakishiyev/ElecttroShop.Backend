using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Sales.Queries.ExportMonthlySalesExcel;

public record ExportMonthlySalesExcelQuery(int Year, int Month)
    : IRequest<Result<SalesExportFileDto>>;
