using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Brands.Commands.CreateBrand;

public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, Result<BrandDto>>
{
    private readonly IWriteRepository<Brand> _brandRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILookupCacheInvalidator _lookupCacheInvalidator;

    public CreateBrandCommandHandler(
        IWriteRepository<Brand> brandRepository,
        IUnitOfWork unitOfWork,
        ILookupCacheInvalidator lookupCacheInvalidator)
    {
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
        _lookupCacheInvalidator = lookupCacheInvalidator;
    }

    public async Task<Result<BrandDto>> Handle(
        CreateBrandCommand request,
        CancellationToken cancellationToken)
    {
        Brand brand;
        try
        {
            brand = Brand.Create(request.Name);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<BrandDto>(Error.Validation("Brand.InvalidData", ex.Message));
        }

        await _brandRepository.AddAsync(brand, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _lookupCacheInvalidator.InvalidateBrandsLookup();

        var brandDto = new BrandDto
        {
            Id = brand.Id,
            Name = brand.Name,
            CreatedAt = brand.CreatedAtUtc
        };

        return Result.Success(brandDto);
    }
}

