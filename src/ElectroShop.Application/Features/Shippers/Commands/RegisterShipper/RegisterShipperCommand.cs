using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Shippers.Commands.RegisterShipper;

public record RegisterShipperCommand(
    string FullName,
    string Email,
    string Password,
    Guid ForwardingFreightId,
    string? Phone = null,
    string? Address = null) : IRequest<Result<ShipperDto>>;

