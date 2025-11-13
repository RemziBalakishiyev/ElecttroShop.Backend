using ElectroShop.Domain.Primitives;

namespace ElectroShop.Domain.Entities;

public class Customer : BaseCommonEntity
{
    public string FullName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string? Phone { get; private set; }
    public string? PasswordHash { get; private set; }

    private Customer() { }

    private Customer(string fullName, string email, string? phone = null)
    {
        FullName = fullName;
        Email = email;
        Phone = phone;
    }

    public static Customer Create(string fullName, string email, string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Müştərinin tam adı boş ola bilməz", nameof(fullName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-poçt ünvanı boş ola bilməz", nameof(email));

        var emailLower = email.Trim().ToLowerInvariant();
        if (!IsValidEmail(emailLower))
            throw new ArgumentException("Yanlış e-poçt ünvanı formatı", nameof(email));

        return new Customer(fullName.Trim(), emailLower, phone?.Trim());
    }

    public void Update(string fullName, string email, string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Müştərinin tam adı boş ola bilməz", nameof(fullName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-poçt ünvanı boş ola bilməz", nameof(email));

        var emailLower = email.Trim().ToLowerInvariant();
        if (!IsValidEmail(emailLower))
            throw new ArgumentException("Yanlış e-poçt ünvanı formatı", nameof(email));

        FullName = fullName.Trim();
        Email = emailLower;
        Phone = phone?.Trim();
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
