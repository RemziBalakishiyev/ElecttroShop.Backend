using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Shippers.Queries.GetShippersByForwardingFreight;

public class GetShippersByForwardingFreightQueryHandler : IRequestHandler<GetShippersByForwardingFreightQuery, Result<List<ShipperDto>>>
{
    private readonly IShipperQueryRepository _shipperRepository;
    private readonly IQueryRepository<Domain.Entities.ForwardingFreight> _forwardingFreightRepository;

    public GetShippersByForwardingFreightQueryHandler(
        IShipperQueryRepository shipperRepository,
        IQueryRepository<Domain.Entities.ForwardingFreight> forwardingFreightRepository)
    {
        _shipperRepository = shipperRepository;
        _forwardingFreightRepository = forwardingFreightRepository;
    }

    public async Task<Result<List<ShipperDto>>> Handle(
        GetShippersByForwardingFreightQuery request,
        CancellationToken cancellationToken)
    {
        // FF-nin mövcud olduğunu yoxla
        var forwardingFreight = await _forwardingFreightRepository.GetByIdAsync(request.ForwardingFreightId, cancellationToken);
        if (forwardingFreight == null)
        {
            return Result.Failure<List<ShipperDto>>(Error.NotFound("ForwardingFreight.NotFound", "Forwarding Freight tapılmadı"));
        }

        // FF-ə bağlı bütün shipperləri gətir
        var shippers = await _shipperRepository.GetShippersByForwardingFreightIdAsync(request.ForwardingFreightId, cancellationToken);

        var shipperDtos = shippers.Select(s => new ShipperDto
        {
            Id = s.Id,
            FullName = s.FullName,
            Email = s.Email,
            Phone = s.Phone,
            Address = s.Address,
            IsActive = s.IsActive,
            ForwardingFreightId = s.ForwardingFreightId,
            ForwardingFreightCompanyName = forwardingFreight.CompanyName,
            CreatedAt = s.CreatedAtUtc,
            UpdatedAt = s.UpdatedAtUtc
        }).ToList();

        return Result.Success(shipperDtos);
    }
}

