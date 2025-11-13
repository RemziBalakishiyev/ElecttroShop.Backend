using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Brands.Commands.UpdateBrand;

public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, Result<BrandDto>>
{
    private readonly IWriteRepository<Brand> _brandRepository;
    private readonly IQueryRepository<Brand> _brandQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBrandCommandHandler(
        IWriteRepository<Brand> brandRepository,
        IQueryRepository<Brand> brandQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _brandRepository = brandRepository;
        _brandQueryRepository = brandQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BrandDto>> Handle(
        UpdateBrandCommand request,
        CancellationToken cancellationToken)
    {
        var brand = await _brandQueryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (brand is null)
        {
            return DomainErrors.Brand.NotFound(request.Id);
        }

        try
        {
            brand.Update(request.Name);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<BrandDto>(Error.Validation("Brand.InvalidData", ex.Message));
        }

        _brandRepository.Update(brand);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var brandDto = new BrandDto
        {
            Id = brand.Id,
            Name = brand.Name,
            CreatedAt = brand.CreatedAtUtc
        };

        return Result.Success(brandDto);
    }
}

