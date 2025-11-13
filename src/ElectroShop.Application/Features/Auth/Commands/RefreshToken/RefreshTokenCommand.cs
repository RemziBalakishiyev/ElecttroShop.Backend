using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<RefreshTokenResponseDto>>;

