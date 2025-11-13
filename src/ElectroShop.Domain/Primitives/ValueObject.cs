using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroShop.Domain.Primitives;
public abstract class ValueObject : IEquatable<ValueObject>
{
    // 🔹 Bütün ValueObject-lər öz sahələrini qaytaran bu metodu implement etməlidir
    protected abstract IEnumerable<object?> GetEqualityComponents();

    // 🔹 Equals override (dəyərləri müqayisə edir)
    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    // 🔹 IEquatable implementasiyası
    public bool Equals(ValueObject? other)
    {
        if (other is null) return false;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    // 🔹 HashCode hesablanması (EF və kolleksiyalar üçün)
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(1, (current, obj) =>
            {
                unchecked
                {
                    return current * 23 + (obj?.GetHashCode() ?? 0);
                }
            });
    }

    // 🔹 Operatorlar (== və !=)
    public static bool operator ==(ValueObject? a, ValueObject? b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }

    public static bool operator !=(ValueObject? a, ValueObject? b) => !(a == b);
}
