using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using ElectroShop.Domain.Entities;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.ProductRatings.Commands.CreateProductRating;

public class CreateProductRatingCommandHandler : IRequestHandler<CreateProductRatingCommand, Result<ProductRatingResponse>>
{
    private readonly IProductQueryRepository _productRepository;
    private readonly IProductRatingQueryRepository _ratingRepository;
    private readonly IWriteRepository<ProductRating> _ratingWriteRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductRatingCommandHandler(
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
        CreateProductRatingCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
            return Result.Failure<ProductRatingResponse>(DomainErrors.Authentication.Unauthorized);

        var userId = _currentUserService.UserId.Value;

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return DomainErrors.Product.NotFound(request.ProductId);

        var existingRating = await _ratingRepository.GetByProductAndUserAsync(
            request.ProductId,
            userId,
            includeDeleted: true,
            cancellationToken);

        if (existingRating is not null && !existingRating.IsDeleted)
            return DomainErrors.ProductRating.AlreadyExists(request.ProductId);

        ProductRating rating;
        try
        {
            if (existingRating is not null && existingRating.IsDeleted)
            {
                existingRating.Restore(request.RatingValue, request.Comment);
                rating = existingRating;
                _ratingWriteRepository.Update(rating);
            }
            else
            {
                rating = ProductRating.Create(request.ProductId, userId, request.RatingValue, request.Comment);
                await _ratingWriteRepository.AddAsync(rating, cancellationToken);
            }
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<ProductRatingResponse>(Error.Validation("ProductRating.InvalidData", ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var savedRating = await _ratingRepository.GetByProductAndUserAsync(
            request.ProductId,
            userId,
            cancellationToken: cancellationToken);

        return Result.Success(savedRating!.Adapt<ProductRatingResponse>());
    }
}
