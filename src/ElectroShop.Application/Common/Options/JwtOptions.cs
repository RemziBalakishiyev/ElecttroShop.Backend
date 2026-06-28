namespace ElectroShop.Application.Common.Options;

/// <summary>
/// JWT Configuration Options
/// </summary>
public class JwtOptions
{
    public const string SectionName = "JWT";

    /// <summary>Primary signing key (env: JWT__Key).</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>Legacy signing key (env: JWT__SecretKey). Used when Key is empty.</summary>
    public string SecretKey { get; set; } = string.Empty;

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; } = 60;
    public int RefreshTokenExpirationDays { get; set; } = 30;

    public string SigningKey => !string.IsNullOrWhiteSpace(Key) ? Key : SecretKey;
}

