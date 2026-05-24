using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Shippers.Commands.CreateShipperByFF;

public class CreateShipperByFFCommandHandler : IRequestHandler<CreateShipperByFFCommand, Result<ShipperDto>>
{
    private readonly IWriteRepository<Shipper> _shipperRepository;
    private readonly IQueryRepository<ForwardingFreight> _forwardingFreightRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateShipperByFFCommandHandler(
        IWriteRepository<Shipper> shipperRepository,
        IQueryRepository<ForwardingFreight> forwardingFreightRepository,
        IUnitOfWork unitOfWork)
    {
        _shipperRepository = shipperRepository;
        _forwardingFreightRepository = forwardingFreightRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ShipperDto>> Handle(
        CreateShipperByFFCommand request,
        CancellationToken cancellationToken)
    {
        // FF-nin mövcud olduğunu yoxla
        var forwardingFreight = await _forwardingFreightRepository.GetByIdAsync(request.ForwardingFreightId, cancellationToken);
        if (forwardingFreight == null)
        {
            return Result.Failure<ShipperDto>(Error.NotFound("ForwardingFreight.NotFound", "Forwarding Freight tapılmadı"));
        }

        if (!forwardingFreight.IsActive)
        {
            return Result.Failure<ShipperDto>(Error.Validation("ForwardingFreight.Inactive", "Forwarding Freight aktiv deyil"));
        }

        Shipper shipper;
        try
        {
            shipper = Shipper.CreateByForwardingFreight(
                request.FullName,
                request.Email,
                request.ForwardingFreightId,
                request.Phone,
                request.Address);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<ShipperDto>(Error.Validation("Shipper.InvalidData", ex.Message));
        }

        await _shipperRepository.AddAsync(shipper, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var shipperDto = new ShipperDto
        {
            Id = shipper.Id,
            FullName = shipper.FullName,
            Email = shipper.Email,
            Phone = shipper.Phone,
            Address = shipper.Address,
            IsActive = shipper.IsActive,
            ForwardingFreightId = shipper.ForwardingFreightId,
            ForwardingFreightCompanyName = forwardingFreight.CompanyName,
            CreatedAt = shipper.CreatedAtUtc,
            UpdatedAt = shipper.UpdatedAtUtc
        };

        return Result.Success(shipperDto);
    }
}

