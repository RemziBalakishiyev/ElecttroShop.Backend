using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Brands.Queries.GetBrandById;

public class GetBrandByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, Result<BrandDto>>
{
    private readonly IQueryRepository<Domain.Entities.Brand> _brandRepository;

    public GetBrandByIdQueryHandler(IQueryRepository<Domain.Entities.Brand> brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<Result<BrandDto>> Handle(
        GetBrandByIdQuery request,
        CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.FirstOrDefaultAsync(
            b => b.Id == request.Id && !b.IsDeleted,
            cancellationToken);

        if (brand is null)
        {
            return DomainErrors.Brand.NotFound(request.Id);
        }

        var brandDto = new BrandDto
        {
            Id = brand.Id,
            Name = brand.Name,
            CreatedAt = brand.CreatedAtUtc
        };

        return Result.Success(brandDto);
    }
}

