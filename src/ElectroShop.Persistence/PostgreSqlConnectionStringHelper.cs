namespace ElectroShop.Persistence;

/// <summary>
/// PostgreSQL connection string-i Npgsql üçün uyğunlaşdırır.
/// SQL Server parametrleri (məs. Encrypt) silinir.
/// </summary>
public static class PostgreSqlConnectionStringHelper
{
    private static readonly HashSet<string> UnsupportedKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "Encrypt",
        "TrustServerCertificate",
        "MultipleActiveResultSets",
        "Integrated Security",
        "Persist Security Info",
        "User Instance",
        "AttachDbFilename"
    };

    public static string Normalize(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return string.Empty;

        var trimmed = connectionString.Trim();

        if (trimmed.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) ||
            trimmed.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
        {
            return trimmed;
        }

        var parts = trimmed.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var kept = new List<string>(parts.Length);

        foreach (var part in parts)
        {
            var separatorIndex = part.IndexOf('=');
            if (separatorIndex <= 0)
            {
                kept.Add(part);
                continue;
            }

            var key = part[..separatorIndex].Trim();
            if (!UnsupportedKeys.Contains(key))
                kept.Add(part);
        }

        return string.Join(';', kept);
    }
}
