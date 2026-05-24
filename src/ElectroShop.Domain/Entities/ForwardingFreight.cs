using ElectroShop.Domain.Primitives;

namespace ElectroShop.Domain.Entities;

public class ForwardingFreight : BaseCommonEntity
{
    public string CompanyName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string? Phone { get; private set; }
    public string? Address { get; private set; }
    public string? TaxId { get; private set; }
    public bool IsActive { get; private set; } = true;
    
    // Navigation property
    public List<Shipper> Shippers { get; private set; } = [];

    private ForwardingFreight() { }

    private ForwardingFreight(string companyName, string email, string? phone = null, string? address = null, string? taxId = null)
    {
        CompanyName = companyName;
        Email = email;
        Phone = phone;
        Address = address;
        TaxId = taxId;
        IsActive = true;
    }

    public static ForwardingFreight Create(string companyName, string email, string? phone = null, string? address = null, string? taxId = null)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new ArgumentException("Şirkət adı boş ola bilməz", nameof(companyName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-poçt ünvanı boş ola bilməz", nameof(email));

        var emailLower = email.Trim().ToLowerInvariant();
        if (!IsValidEmail(emailLower))
            throw new ArgumentException("Yanlış e-poçt ünvanı formatı", nameof(email));

        return new ForwardingFreight(companyName.Trim(), emailLower, phone?.Trim(), address?.Trim(), taxId?.Trim());
    }

    public void Update(string companyName, string email, string? phone = null, string? address = null, string? taxId = null)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new ArgumentException("Şirkət adı boş ola bilməz", nameof(companyName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-poçt ünvanı boş ola bilməz", nameof(email));

        var emailLower = email.Trim().ToLowerInvariant();
        if (!IsValidEmail(emailLower))
            throw new ArgumentException("Yanlış e-poçt ünvanı formatı", nameof(email));

        CompanyName = companyName.Trim();
        Email = emailLower;
        Phone = phone?.Trim();
        Address = address?.Trim();
        TaxId = taxId?.Trim();
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
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

