using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.SetProductAsBanner;

public class SetProductAsBannerCommandHandler : IRequestHandler<SetProductAsBannerCommand, Result>
{
    private readonly IQueryRepository<Product> _productQueryRepository;
    private readonly IWriteRepository<Product> _productWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetProductAsBannerCommandHandler(
        IQueryRepository<Product> productQueryRepository,
        IWriteRepository<Product> productWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _productQueryRepository = productQueryRepository;
        _productWriteRepository = productWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SetProductAsBannerCommand request, CancellationToken cancellationToken)
    {
        var product = await _productQueryRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return Result.Failure(DomainErrors.Product.NotFound(request.ProductId));
        }

        // Əgər başqa bir məhsul banner-dırsa, onu çıxar
        var existingBanner = await _productQueryRepository.FirstOrDefaultAsync(
            p => p.IsBanner == true && p.Id != request.ProductId,
            cancellationToken);

        if (existingBanner != null)
        {
            existingBanner.RemoveFromBanner();
            _productWriteRepository.Update(existingBanner);
        }

        product.SetAsBanner();
        _productWriteRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

