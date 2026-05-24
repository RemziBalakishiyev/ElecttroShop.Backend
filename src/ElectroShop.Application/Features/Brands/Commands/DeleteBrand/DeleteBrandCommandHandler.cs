using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.Services;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Brands.Commands.DeleteBrand;

public class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand, Result>
{
    private readonly IWriteRepository<Brand> _brandRepository;
    private readonly IQueryRepository<Brand> _brandQueryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILookupCacheInvalidator _lookupCacheInvalidator;

    public DeleteBrandCommandHandler(
        IWriteRepository<Brand> brandRepository,
        IQueryRepository<Brand> brandQueryRepository,
        IUnitOfWork unitOfWork,
        ILookupCacheInvalidator lookupCacheInvalidator)
    {
        _brandRepository = brandRepository;
        _brandQueryRepository = brandQueryRepository;
        _unitOfWork = unitOfWork;
        _lookupCacheInvalidator = lookupCacheInvalidator;
    }

    public async Task<Result> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await _brandQueryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (brand is null)
        {
            return Result.Failure(DomainErrors.Brand.NotFound(request.Id));
        }

        brand.MarkDeleted();

        _brandRepository.Update(brand);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _lookupCacheInvalidator.InvalidateBrandsLookup();

        return Result.Success();
    }
}

