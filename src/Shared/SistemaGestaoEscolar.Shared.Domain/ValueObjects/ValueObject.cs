namespace SistemaGestaoEscolar.Shared.Domain.ValueObjects;

public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;

        var other = (ValueObject)obj;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        return left?.Equals(right) ?? right is null;
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }

    protected static bool EqualOperator(ValueObject? left, ValueObject? right)
    {
        if (left is null ^ right is null)
            return false;

        return left?.Equals(right) != false;
    }

    protected static bool NotEqualOperator(ValueObject? left, ValueObject? right)
    {
        return !EqualOperator(left, right);
    }
}