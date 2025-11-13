using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password) : IRequest<Result<LoginResponseDto>>;

