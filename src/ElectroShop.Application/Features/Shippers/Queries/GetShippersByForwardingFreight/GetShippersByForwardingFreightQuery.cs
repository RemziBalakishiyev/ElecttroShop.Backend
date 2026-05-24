using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Shippers.Queries.GetShippersByForwardingFreight;

public record GetShippersByForwardingFreightQuery(
    Guid ForwardingFreightId) : IRequest<Result<List<ShipperDto>>>;

