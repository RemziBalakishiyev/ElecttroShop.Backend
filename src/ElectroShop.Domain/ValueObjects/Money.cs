using ElectroShop.Domain.Primitives;

namespace ElectroShop.Domain.ValueObjects;

public sealed class Money : ValueObject
{
    public static readonly string[] ValidCurrencies = { "TRY", "USD", "EUR", "AZN", "GBP" };

    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    private Money() { 
        Currency = string.Empty;
    } // EF üçün

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("Məbləğ mənfi ola bilməz", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Valyuta boş ola bilməz", nameof(currency));

        var normalizedCurrency = currency.ToUpperInvariant();
        if (!ValidCurrencies.Contains(normalizedCurrency))
            throw new ArgumentException($"Dəstəklənməyən valyuta: {currency}", nameof(currency));

        Amount = amount;
        Currency = normalizedCurrency;
    }

    public static Money Zero(string currency = "AZN") => new(0m, currency);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Valyutalar uyğun gəlmir");
        
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Valyutalar uyğun gəlmir");
        
        return new Money(Amount - other.Amount, Currency);
    }

    public override string ToString() => $"{Amount:N2} {Currency}";
}


