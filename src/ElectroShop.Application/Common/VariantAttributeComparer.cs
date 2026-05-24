using System.Text.Json;

namespace ElectroShop.Application.Common;

public static class VariantAttributeComparer
{
    public static string CreateFingerprint(IReadOnlyDictionary<string, string> canonicalAttributes)
    {
        var sorted = canonicalAttributes
            .OrderBy(kvp => kvp.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return JsonSerializer.Serialize(sorted);
    }

    public static bool AreEquivalent(
        IReadOnlyDictionary<string, string> left,
        IReadOnlyDictionary<string, string> right)
    {
        return string.Equals(CreateFingerprint(left), CreateFingerprint(right), StringComparison.Ordinal);
    }
}