using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Shippers.Commands.CreateShipperByFF;

public record CreateShipperByFFCommand(
    string FullName,
    string Email,
    Guid ForwardingFreightId,
    string? Phone = null,
    string? Address = null) : IRequest<Result<ShipperDto>>;

