using ElectroShop.Domain.Enums;
using ElectroShop.Domain.Primitives;

namespace ElectroShop.Domain.Entities;

public class User : BaseCommonEntity
{
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string FullName { get; private set; } = default!;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; } = true;

    private User() { }

    private User(string email, string passwordHash, string fullName, UserRole role)
    {
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
        Role = role;
        IsActive = true;
    }

    public static User Create(string email, string passwordHash, string fullName, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-poçt ünvanı boş ola bilməz", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Şifrə hash-i boş ola bilməz", nameof(passwordHash));

        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Tam ad boş ola bilməz", nameof(fullName));

        var emailLower = email.Trim().ToLowerInvariant();
        if (!IsValidEmail(emailLower))
            throw new ArgumentException("Yanlış e-poçt ünvanı formatı", nameof(email));

        return new User(emailLower, passwordHash, fullName.Trim(), role);
    }

    public void Update(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Tam ad boş ola bilməz", nameof(fullName));

        FullName = fullName.Trim();
    }

    public void ChangePassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Şifrə hash-i boş ola bilməz", nameof(passwordHash));

        PasswordHash = passwordHash;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public bool VerifyPassword(string passwordHash)
    {
        return PasswordHash == passwordHash;
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

