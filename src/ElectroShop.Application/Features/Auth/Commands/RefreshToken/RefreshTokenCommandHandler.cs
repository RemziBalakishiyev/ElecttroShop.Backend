using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponseDto>>
{
    private readonly IQueryRepository<Domain.Entities.RefreshToken> _refreshTokenRepository;
    private readonly IQueryRepository<User> _userRepository;
    private readonly IWriteRepository<Domain.Entities.RefreshToken> _refreshTokenWriteRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IQueryRepository<Domain.Entities.RefreshToken> refreshTokenRepository,
        IQueryRepository<User> userRepository,
        IWriteRepository<Domain.Entities.RefreshToken> refreshTokenWriteRepository,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _refreshTokenWriteRepository = refreshTokenWriteRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RefreshTokenResponseDto>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokenRepository.FirstOrDefaultAsync(
            rt => rt.Token == request.RefreshToken && !rt.IsUsed && !rt.IsRevoked,
            cancellationToken);

        if (refreshToken is null)
        {
            return Result.Failure<RefreshTokenResponseDto>(
                Error.Unauthorized("Auth.InvalidRefreshToken", "Yanlış və ya istifadə olunmuş refresh token"));
        }

        if (refreshToken.IsExpired())
        {
            refreshToken.Revoke();
            _refreshTokenWriteRepository.Update(refreshToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Failure<RefreshTokenResponseDto>(
                Error.Unauthorized("Auth.ExpiredRefreshToken", "Refresh token müddəti bitib"));
        }

        var user = await _userRepository.GetByIdAsync(refreshToken.UserId, cancellationToken);
        if (user is null || !user.IsActive || user.IsDeleted)
        {
            return Result.Failure<RefreshTokenResponseDto>(
                Error.Unauthorized("Auth.UserNotFound", "İstifadəçi tapılmadı və ya deaktivdir"));
        }

        try
        {
            refreshToken.MarkAsUsed();
            _refreshTokenWriteRepository.Update(refreshToken);

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var expiresAt = _tokenService.GetAccessTokenExpiration();

            var newRefreshTokenEntity = Domain.Entities.RefreshToken.Create(
                user.Id,
                newRefreshToken,
                DateTime.UtcNow.AddDays(30));

            await _refreshTokenWriteRepository.AddAsync(newRefreshTokenEntity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new RefreshTokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = expiresAt
            };

            return Result.Success(response);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<RefreshTokenResponseDto>(
                Error.Validation("Auth.InvalidOperation", ex.Message));
        }
    }
}

