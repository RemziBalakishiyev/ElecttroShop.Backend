using ElectroShop.Domain.Primitives;
using System.Text.RegularExpressions;

namespace ElectroShop.Domain.ValueObjects;

public sealed class Sku : ValueObject
{
    public const int MinLength = 3;
    public const int MaxLength = 50;
    public static readonly Regex SkuPattern = new(@"^[A-Z0-9\-_]+$", RegexOptions.Compiled);

    public string Value { get; private set; }

    private Sku() {
        Value = string.Empty;
    } // EF üçün

    public Sku(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("SKU boş ola bilməz", nameof(value));

        var normalizedValue = value.Trim().ToUpperInvariant();

        if (normalizedValue.Length < MinLength)
            throw new ArgumentException($"SKU minimum {MinLength} simvol olmalıdır", nameof(value));

        if (normalizedValue.Length > MaxLength)
            throw new ArgumentException($"SKU maksimum {MaxLength} simvol ola bilər", nameof(value));

        if (!SkuPattern.IsMatch(normalizedValue))
            throw new ArgumentException("SKU yalnız böyük hərflər, rəqəmlər, tire və alt xətt simvollarından ibarət ola bilər", nameof(value));

        Value = normalizedValue;
    }

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
