using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Sales.Commands.CreateSale;

public record CreateSaleCommand(
    Guid? ProductId,
    string? ProductName,
    string? ProductCode,
    Guid? CategoryId,
    string? CategoryName,
    decimal? CostPrice,
    decimal SalePrice,
    int Quantity,
    DateTime? SoldAt,
    string? Note) : IRequest<Result<SaleDetailDto>>;
