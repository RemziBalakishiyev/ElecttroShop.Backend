using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Sales.Commands.UpdateSale;

public record UpdateSaleCommand(
    Guid Id,
    string? ProductName,
    string? ProductCode,
    Guid? CategoryId,
    string? CategoryName,
    decimal? CostPrice,
    decimal SalePrice,
    int Quantity,
    DateTime? SoldAt,
    string? Note,
    IReadOnlyList<SaleExpenseRequestDto>? Expenses = null) : IRequest<Result<SaleDetailDto>>;
