using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.SetProductAsPopular;

public class SetProductAsPopularCommandHandler : IRequestHandler<SetProductAsPopularCommand, Result>
{
    private readonly IQueryRepository<Product> _productQueryRepository;
    private readonly IWriteRepository<Product> _productWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetProductAsPopularCommandHandler(
        IQueryRepository<Product> productQueryRepository,
        IWriteRepository<Product> productWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _productQueryRepository = productQueryRepository;
        _productWriteRepository = productWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SetProductAsPopularCommand request, CancellationToken cancellationToken)
    {
        var product = await _productQueryRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return Result.Failure(DomainErrors.Product.NotFound(request.ProductId));
        }

        var existingPopular = await _productQueryRepository.FirstOrDefaultAsync(
            p => p.IsPopular && p.PopularDisplayOrder == request.DisplayOrder && p.Id != request.ProductId,
            cancellationToken);

        if (existingPopular != null)
        {
            existingPopular.RemoveFromPopular();
            _productWriteRepository.Update(existingPopular);
        }

        try
        {
            product.SetAsPopular(request.DisplayOrder);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(Error.Validation("Product.InvalidPopularDisplayOrder", ex.Message));
        }

        _productWriteRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
