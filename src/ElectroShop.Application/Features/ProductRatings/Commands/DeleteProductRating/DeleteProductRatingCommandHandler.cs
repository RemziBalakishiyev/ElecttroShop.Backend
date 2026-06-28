using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.Services;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.ProductRatings.Commands.DeleteProductRating;

public class DeleteProductRatingCommandHandler : IRequestHandler<DeleteProductRatingCommand, Result>
{
    private readonly IProductQueryRepository _productRepository;
    private readonly IProductRatingQueryRepository _ratingRepository;
    private readonly IWriteRepository<ProductRating> _ratingWriteRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductRatingCommandHandler(
        IProductQueryRepository productRepository,
        IProductRatingQueryRepository ratingRepository,
        IWriteRepository<ProductRating> ratingWriteRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _ratingRepository = ratingRepository;
        _ratingWriteRepository = ratingWriteRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteProductRatingCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
            return Result.Failure(DomainErrors.Authentication.Unauthorized);

        var userId = _currentUserService.UserId.Value;

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return Result.Failure(DomainErrors.Product.NotFound(request.ProductId));

        var rating = await _ratingRepository.GetByProductAndUserAsync(
            request.ProductId,
            userId,
            cancellationToken: cancellationToken);

        if (rating is null)
            return Result.Failure(DomainErrors.ProductRating.NotFound(request.ProductId));

        rating.MarkDeleted();
        _ratingWriteRepository.Update(rating);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
