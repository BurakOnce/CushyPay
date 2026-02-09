using CushyPay.Domain.Common;
using CushyPay.Domain.Enums;
using CushyPay.Domain.ValueObjects;

namespace CushyPay.Domain.Entities;

public class Wallet : BaseEntity
{
    public int UserId { get; private set; }
    public string Iban { get; private set; } = string.Empty;
    public Currency Currency { get; private set; }
    public decimal Balance { get; private set; }
    public string Name { get; private set; } = string.Empty; // e.g., "Main Account", "Savings", "Dollar Account"
    public bool IsActive { get; private set; }

    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public User User { get; private set; } = null!;
    public ICollection<Transaction> OutgoingTransactions { get; private set; } = new List<Transaction>();

    private Wallet() { }

    private Wallet(int userId, string iban, Currency currency, string name)
    {
        UserId = userId;
        Iban = iban;
        Currency = currency;
        Balance = 0;
        Name = name;
        IsActive = true;
    }

    public static Wallet Create(int userId, Currency currency, string name)
    {
        if (userId <= 0)
            throw new ArgumentException("User ID must be greater than zero.", nameof(userId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Wallet name cannot be null or empty.", nameof(name));

        var iban = ValueObjects.Iban.Generate();
        return new Wallet(userId, iban.Value, currency, name);
    }

    public void Credit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        Balance += amount;
        MarkAsUpdated();
    }

    public void Debit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        if (Balance < amount)
            throw new InvalidOperationException("Insufficient balance.");

        Balance -= amount;
        MarkAsUpdated();
    }

    public bool HasSufficientBalance(decimal amount)
    {
        return Balance >= amount;
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Wallet name cannot be null or empty.", nameof(name));

        Name = name;
        MarkAsUpdated();
    }
}

