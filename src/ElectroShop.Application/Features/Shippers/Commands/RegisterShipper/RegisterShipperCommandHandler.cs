using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Shippers.Commands.RegisterShipper;

public class RegisterShipperCommandHandler : IRequestHandler<RegisterShipperCommand, Result<ShipperDto>>
{
    private readonly IWriteRepository<Shipper> _shipperRepository;
    private readonly IQueryRepository<ForwardingFreight> _forwardingFreightRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterShipperCommandHandler(
        IWriteRepository<Shipper> shipperRepository,
        IQueryRepository<ForwardingFreight> forwardingFreightRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _shipperRepository = shipperRepository;
        _forwardingFreightRepository = forwardingFreightRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ShipperDto>> Handle(
        RegisterShipperCommand request,
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

        // Şifrəni hash et
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        Shipper shipper;
        try
        {
            shipper = Shipper.CreateSelfRegistration(
                request.FullName,
                request.Email,
                request.ForwardingFreightId,
                passwordHash,
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

