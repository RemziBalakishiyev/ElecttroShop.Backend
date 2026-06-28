using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using ElectroShop.Domain.Entities;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.ProductRatings.Commands.UpdateProductRating;

public class UpdateProductRatingCommandHandler : IRequestHandler<UpdateProductRatingCommand, Result<ProductRatingResponse>>
{
    private readonly IProductQueryRepository _productRepository;
    private readonly IProductRatingQueryRepository _ratingRepository;
    private readonly IWriteRepository<ProductRating> _ratingWriteRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductRatingCommandHandler(
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

    public async Task<Result<ProductRatingResponse>> Handle(
        UpdateProductRatingCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
            return Result.Failure<ProductRatingResponse>(DomainErrors.Authentication.Unauthorized);

        var userId = _currentUserService.UserId.Value;

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return DomainErrors.Product.NotFound(request.ProductId);

        var rating = await _ratingRepository.GetByProductAndUserAsync(
            request.ProductId,
            userId,
            cancellationToken: cancellationToken);

        if (rating is null)
            return DomainErrors.ProductRating.NotFound(request.ProductId);

        try
        {
            rating.Update(request.RatingValue, request.Comment);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<ProductRatingResponse>(Error.Validation("ProductRating.InvalidData", ex.Message));
        }

        _ratingWriteRepository.Update(rating);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedRating = await _ratingRepository.GetByProductAndUserAsync(
            request.ProductId,
            userId,
            cancellationToken: cancellationToken);

        return Result.Success(updatedRating!.Adapt<ProductRatingResponse>());
    }
}
