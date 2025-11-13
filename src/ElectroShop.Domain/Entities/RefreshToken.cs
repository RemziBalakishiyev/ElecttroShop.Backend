using ElectroShop.Domain.Primitives;

namespace ElectroShop.Domain.Entities;

public class RefreshToken : BaseCommonEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;
    public string Token { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public bool IsUsed { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    private RefreshToken() { }

    private RefreshToken(Guid userId, string token, DateTime expiresAt)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        IsRevoked = false;
        IsUsed = false;
    }

    public static RefreshToken Create(Guid userId, string token, DateTime expiresAt)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("İstifadəçi ID-si boş ola bilməz", nameof(userId));

        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token boş ola bilməz", nameof(token));

        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Token bitmə vaxtı keçmişə aid ola bilməz", nameof(expiresAt));

        return new RefreshToken(userId, token, expiresAt);
    }

    public void MarkAsUsed()
    {
        if (IsUsed)
            throw new InvalidOperationException("Token artıq istifadə edilib");

        if (IsRevoked)
            throw new InvalidOperationException("Token ləğv edilib");

        if (IsExpired())
            throw new InvalidOperationException("Token müddəti bitib");

        IsUsed = true;
    }

    public void Revoke()
    {
        if (IsRevoked)
            return;

        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow >= ExpiresAt;
    }

    public bool IsValid()
    {
        return !IsRevoked && !IsUsed && !IsExpired();
    }
}

