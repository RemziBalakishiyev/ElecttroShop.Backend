using ElectroShop.Domain.Enums;

namespace ElectroShop.Application.DTOs;

/// <summary>
/// Login Response DTO
/// </summary>
public record LoginResponseDto
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public UserDto User { get; init; } = default!;
}

/// <summary>
/// Refresh Token Response DTO
/// </summary>
public record RefreshTokenResponseDto
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}

/// <summary>
/// User DTO
/// </summary>
public record UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public UserRole Role { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}

