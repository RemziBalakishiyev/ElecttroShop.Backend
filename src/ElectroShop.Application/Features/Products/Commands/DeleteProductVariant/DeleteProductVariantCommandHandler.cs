using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.DeleteProductVariant;

public class DeleteProductVariantCommandHandler 
    : IRequestHandler<DeleteProductVariantCommand, Result>
{
    private readonly IWriteRepository<Domain.Entities.ProductVariant> _variantRepository;
    private readonly IQueryRepository<Domain.Entities.ProductVariant> _variantQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductVariantCommandHandler(
        IWriteRepository<Domain.Entities.ProductVariant> variantRepository,
        IQueryRepository<Domain.Entities.ProductVariant> variantQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _variantRepository = variantRepository;
        _variantQueryRepository = variantQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteProductVariantCommand request,
        CancellationToken cancellationToken)
    {
        var variant = await _variantQueryRepository.GetByIdAsync(request.VariantId, cancellationToken);
        if (variant is null || variant.ProductId != request.ProductId)
        {
            return Result.Failure(DomainErrors.Product.NotFound(request.VariantId));
        }

        variant.Deactivate();
        _variantRepository.Update(variant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

