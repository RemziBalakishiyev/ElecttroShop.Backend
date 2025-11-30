using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.SetProductAsFeatured;

public class SetProductAsFeaturedCommandHandler : IRequestHandler<SetProductAsFeaturedCommand, Result>
{
    private readonly IQueryRepository<Product> _productQueryRepository;
    private readonly IWriteRepository<Product> _productWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetProductAsFeaturedCommandHandler(
        IQueryRepository<Product> productQueryRepository,
        IWriteRepository<Product> productWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _productQueryRepository = productQueryRepository;
        _productWriteRepository = productWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SetProductAsFeaturedCommand request, CancellationToken cancellationToken)
    {
        var product = await _productQueryRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return Result.Failure(DomainErrors.Product.NotFound(request.ProductId));
        }

        // Əgər bu display order artıq istifadə olunursa, o məhsulu featured-dan çıxar
        var existingFeatured = await _productQueryRepository.FirstOrDefaultAsync(
            p => p.IsFeatured == true && p.DisplayOrder == request.DisplayOrder && p.Id != request.ProductId,
            cancellationToken);

        if (existingFeatured != null)
        {
            existingFeatured.RemoveFromFeatured();
            _productWriteRepository.Update(existingFeatured);
        }

        try
        {
            product.SetAsFeatured(request.DisplayOrder);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(Error.Validation("Product.InvalidDisplayOrder", ex.Message));
        }

        _productWriteRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

