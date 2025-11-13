namespace ElectroShop.Application.Services;

/// <summary>
/// Password hashing service
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Şifrəni hash edir
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Hash edilmiş şifrə ilə plain text şifrəni müqayisə edir
    /// </summary>
    bool VerifyPassword(string password, string hashedPassword);
}

