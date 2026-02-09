namespace CushyPay.Domain.ValueObjects;

public sealed class Money : IEquatable<Money>
{
    public decimal Amount { get; }
    public Enums.Currency Currency { get; }

    private Money(decimal amount, Enums.Currency currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));

        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, Enums.Currency currency)
    {
        return new Money(amount, currency);
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies.");

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot subtract money with different currencies.");

        if (Amount < other.Amount)
            throw new InvalidOperationException("Insufficient funds.");

        return new Money(Amount - other.Amount, Currency);
    }

    public bool IsGreaterThan(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot compare money with different currencies.");

        return Amount > other.Amount;
    }

    public bool IsLessThan(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot compare money with different currencies.");

        return Amount < other.Amount;
    }

    public bool Equals(Money? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Amount == other.Amount && Currency == other.Currency;
    }

    public override bool Equals(object? obj)
    {
        return obj is Money other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Amount, Currency);
    }

    public override string ToString() => $"{Amount:F2} {Currency}";

    public static Money Zero(Enums.Currency currency) => new(0, currency);
}

