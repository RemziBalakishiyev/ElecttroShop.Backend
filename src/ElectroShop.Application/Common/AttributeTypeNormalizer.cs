namespace ElectroShop.Application.Common;

public static class AttributeTypeNormalizer
{
    public static string Normalize(string attributeType)
    {
        if (string.IsNullOrWhiteSpace(attributeType))
            return string.Empty;

        return attributeType.Trim();
    }

    public static bool Equals(string? a, string? b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;
        return string.Equals(Normalize(a), Normalize(b), StringComparison.OrdinalIgnoreCase);
    }

    public static string GetCanonicalKey(IEnumerable<string> existingTypes, string incoming)
    {
        var normalizedIncoming = Normalize(incoming);
        var match = existingTypes.FirstOrDefault(t => Equals(t, normalizedIncoming));
        return match ?? normalizedIncoming;
    }

    public static string NormalizeValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return value.Trim();
    }

    public static bool ValueEquals(string? a, string? b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;
        return string.Equals(NormalizeValue(a), NormalizeValue(b), StringComparison.Ordinal);
    }
}