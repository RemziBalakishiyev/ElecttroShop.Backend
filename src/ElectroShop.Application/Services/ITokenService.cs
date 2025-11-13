using ElectroShop.Domain.Entities;
using System.Security.Claims;

namespace ElectroShop.Application.Services;

/// <summary>
/// Token generation və validation üçün service
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Access token yaradır
    /// </summary>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Refresh token yaradır
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Token-dan Claims oxuyur
    /// </summary>
    ClaimsPrincipal? GetPrincipalFromToken(string token);

    /// <summary>
    /// Access token bitmə vaxtını qaytarır
    /// </summary>
    DateTime GetAccessTokenExpiration();
}

