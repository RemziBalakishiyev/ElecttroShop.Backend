using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using ElectroShop.Domain.Entities;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    private readonly IQueryRepository<User> _userRepository;
    private readonly IWriteRepository<Domain.Entities.RefreshToken> _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IQueryRepository<User> userRepository,
        IWriteRepository<Domain.Entities.RefreshToken> refreshTokenRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResponseDto>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var emailLower = request.Email.Trim().ToLowerInvariant();

        var user = await _userRepository.FirstOrDefaultAsync(
            u => u.Email == emailLower && !u.IsDeleted,
            cancellationToken);

        if (user is null)
        {
            return DomainErrors.Authentication.InvalidCredentials;
        }

        if (!user.IsActive)
        {
            return Result.Failure<LoginResponseDto>(
                Error.Unauthorized("User.Inactive", "İstifadəçi hesabı deaktivdir"));
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return DomainErrors.Authentication.InvalidCredentials;
        }

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var expiresAt = _tokenService.GetAccessTokenExpiration();

        var refreshTokenEntity = Domain.Entities.RefreshToken.Create(
            user.Id,
            refreshToken,
            DateTime.UtcNow.AddDays(30));

        await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userDto = user.Adapt<UserDto>();

        var response = new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = userDto
        };

        return Result.Success(response);
    }
}

