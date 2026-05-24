using ElectroShop.Domain.Primitives;

namespace ElectroShop.Domain.Entities;

public class Shipper : BaseCommonEntity
{
    public string FullName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string? Phone { get; private set; }
    public string? Address { get; private set; }
    public string? PasswordHash { get; private set; }
    public bool IsActive { get; private set; } = true;
    
    // FF ilə əlaqə - nullable çünki shipper özü qeydiyyatdan keçəndə də ola bilər
    public Guid? ForwardingFreightId { get; private set; }
    public ForwardingFreight? ForwardingFreight { get; private set; }

    private Shipper() { }

    private Shipper(string fullName, string email, string? phone = null, string? address = null, Guid? forwardingFreightId = null)
    {
        FullName = fullName;
        Email = email;
        Phone = phone;
        Address = address;
        ForwardingFreightId = forwardingFreightId;
        IsActive = true;
    }

    /// <summary>
    /// FF tərəfindən shipper yaradılarkən istifadə olunur
    /// </summary>
    public static Shipper CreateByForwardingFreight(string fullName, string email, Guid forwardingFreightId, string? phone = null, string? address = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Göndərənin tam adı boş ola bilməz", nameof(fullName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-poçt ünvanı boş ola bilməz", nameof(email));

        if (forwardingFreightId == Guid.Empty)
            throw new ArgumentException("Forwarding Freight ID boş ola bilməz", nameof(forwardingFreightId));

        var emailLower = email.Trim().ToLowerInvariant();
        if (!IsValidEmail(emailLower))
            throw new ArgumentException("Yanlış e-poçt ünvanı formatı", nameof(email));

        return new Shipper(fullName.Trim(), emailLower, phone?.Trim(), address?.Trim(), forwardingFreightId);
    }

    /// <summary>
    /// Shipper özü qeydiyyatdan keçəndə istifadə olunur - yetkili FF-ə bağlanmalıdır
    /// </summary>
    public static Shipper CreateSelfRegistration(string fullName, string email, Guid forwardingFreightId, string passwordHash, string? phone = null, string? address = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Göndərənin tam adı boş ola bilməz", nameof(fullName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-poçt ünvanı boş ola bilməz", nameof(email));

        if (forwardingFreightId == Guid.Empty)
            throw new ArgumentException("Forwarding Freight ID boş ola bilməz", nameof(forwardingFreightId));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Şifrə hash-i boş ola bilməz", nameof(passwordHash));

        var emailLower = email.Trim().ToLowerInvariant();
        if (!IsValidEmail(emailLower))
            throw new ArgumentException("Yanlış e-poçt ünvanı formatı", nameof(email));

        var shipper = new Shipper(fullName.Trim(), emailLower, phone?.Trim(), address?.Trim(), forwardingFreightId);
        shipper.PasswordHash = passwordHash;
        return shipper;
    }

    public void Update(string fullName, string email, string? phone = null, string? address = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Göndərənin tam adı boş ola bilməz", nameof(fullName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-poçt ünvanı boş ola bilməz", nameof(email));

        var emailLower = email.Trim().ToLowerInvariant();
        if (!IsValidEmail(emailLower))
            throw new ArgumentException("Yanlış e-poçt ünvanı formatı", nameof(email));

        FullName = fullName.Trim();
        Email = emailLower;
        Phone = phone?.Trim();
        Address = address?.Trim();
    }

    public void AssignToForwardingFreight(Guid forwardingFreightId)
    {
        if (forwardingFreightId == Guid.Empty)
            throw new ArgumentException("Forwarding Freight ID boş ola bilməz", nameof(forwardingFreightId));

        ForwardingFreightId = forwardingFreightId;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void ChangePassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Şifrə hash-i boş ola bilməz", nameof(passwordHash));

        PasswordHash = passwordHash;
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

