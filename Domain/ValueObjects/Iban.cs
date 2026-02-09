namespace CushyPay.Domain.ValueObjects;

public sealed class Iban : IEquatable<Iban>
{
    public string Value { get; }

    private Iban(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("IBAN cannot be null or empty.", nameof(value));

        if (value.Length < 15 || value.Length > 34)
            throw new ArgumentException("IBAN must be between 15 and 34 characters.", nameof(value));

        Value = value;
    }

    public static Iban Create(string value)
    {
        return new Iban(value);
    }

    public static Iban Generate(string countryCode = "TR")
    {
        var random = new Random();
        var checkDigits = random.Next(10, 99).ToString("D2");
        var bankCode = "0001"; // CushyPay Bank Code
        var accountNumber = random.Next(100000000, 999999999).ToString("D9");
        var iban = $"{countryCode}{checkDigits}{bankCode}{accountNumber}";
        
        return new Iban(iban);
    }

    public bool Equals(Iban? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is Iban other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString() => Value;

    public static implicit operator string(Iban iban) => iban.Value;
}

