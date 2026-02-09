using CushyPay.Domain.Common;
using CushyPay.Domain.Enums;

namespace CushyPay.Domain.Entities;

public class Transaction : BaseEntity
{
    public int? FromWalletId { get; private set; }
    public int? ToWalletId { get; private set; }
    public string? ExternalAccountNumber { get; private set; } // For external transfers
    public string? ExternalBankName { get; private set; }
    public decimal Amount { get; private set; }
    public Currency Currency { get; private set; }
    public TransactionType Type { get; private set; }
    public TransactionStatus Status { get; private set; }
    public string? Description { get; private set; }
    public string? ReferenceNumber { get; private set; } // Unique transaction reference
    public DateTime? ProcessedAt { get; private set; }
    public string? FailureReason { get; private set; }

    public Wallet? FromWallet { get; private set; }
    public Wallet? ToWallet { get; private set; }

    private Transaction() { }

    private Transaction(
        int? fromWalletId,
        int? toWalletId,
        decimal amount,
        Currency currency,
        TransactionType type,
        string? description = null,
        string? externalAccountNumber = null,
        string? externalBankName = null)
    {
        FromWalletId = fromWalletId;
        ToWalletId = toWalletId;
        Amount = amount;
        Currency = currency;
        Type = type;
        Status = TransactionStatus.Pending;
        Description = description;
        ExternalAccountNumber = externalAccountNumber;
        ExternalBankName = externalBankName;
        ReferenceNumber = GenerateReferenceNumber();
    }

    public static Transaction CreateInternalTransfer(
        int fromWalletId,
        int toWalletId,
        decimal amount,
        Currency currency,
        string? description = null)
    {
        if (fromWalletId <= 0)
            throw new ArgumentException("From wallet ID must be greater than zero.", nameof(fromWalletId));

        if (toWalletId <= 0)
            throw new ArgumentException("To wallet ID must be greater than zero.", nameof(toWalletId));

        if (fromWalletId == toWalletId)
            throw new ArgumentException("Cannot transfer to the same wallet.", nameof(toWalletId));

        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        return new Transaction(fromWalletId, toWalletId, amount, currency, TransactionType.InternalTransfer, description);
    }

    public static Transaction CreateExternalTransfer(
        int fromWalletId,
        string externalAccountNumber,
        string externalBankName,
        decimal amount,
        Currency currency,
        string? description = null)
    {
        if (fromWalletId <= 0)
            throw new ArgumentException("From wallet ID must be greater than zero.", nameof(fromWalletId));

        if (string.IsNullOrWhiteSpace(externalAccountNumber))
            throw new ArgumentException("External account number cannot be null or empty.", nameof(externalAccountNumber));

        if (string.IsNullOrWhiteSpace(externalBankName))
            throw new ArgumentException("External bank name cannot be null or empty.", nameof(externalBankName));

        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        return new Transaction(
            fromWalletId,
            null,
            amount,
            currency,
            TransactionType.ExternalTransfer,
            description,
            externalAccountNumber,
            externalBankName);
    }

    public static Transaction CreateDeposit(
        int toWalletId,
        decimal amount,
        Currency currency,
        string? description = null)
    {
        if (toWalletId <= 0)
            throw new ArgumentException("To wallet ID must be greater than zero.", nameof(toWalletId));

        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        return new Transaction(null, toWalletId, amount, currency, TransactionType.Deposit, description);
    }

    public static Transaction CreateWithdrawal(
        int fromWalletId,
        string externalAccountNumber,
        string externalBankName,
        decimal amount,
        Currency currency,
        string? description = null)
    {
        if (fromWalletId <= 0)
            throw new ArgumentException("From wallet ID must be greater than zero.", nameof(fromWalletId));

        if (string.IsNullOrWhiteSpace(externalAccountNumber))
            throw new ArgumentException("External account number cannot be null or empty.", nameof(externalAccountNumber));

        if (string.IsNullOrWhiteSpace(externalBankName))
            throw new ArgumentException("External bank name cannot be null or empty.", nameof(externalBankName));

        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        return new Transaction(
            fromWalletId,
            null,
            amount,
            currency,
            TransactionType.Withdrawal,
            description,
            externalAccountNumber,
            externalBankName);
    }

    public void MarkAsCompleted()
    {
        if (Status != TransactionStatus.Pending)
            throw new InvalidOperationException("Only pending transactions can be marked as completed.");

        Status = TransactionStatus.Completed;
        ProcessedAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void MarkAsFailed(string reason)
    {
        if (Status != TransactionStatus.Pending)
            throw new InvalidOperationException("Only pending transactions can be marked as failed.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Failure reason cannot be null or empty.", nameof(reason));

        Status = TransactionStatus.Failed;
        FailureReason = reason;
        ProcessedAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void MarkAsCancelled(string? reason = null)
    {
        if (Status != TransactionStatus.Pending)
            throw new InvalidOperationException("Only pending transactions can be cancelled.");

        Status = TransactionStatus.Cancelled;
        FailureReason = reason;
        ProcessedAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    private static string GenerateReferenceNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = new Random();
        var randomPart = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 8)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        
        return $"TXN-{timestamp}-{randomPart}";
    }
}

