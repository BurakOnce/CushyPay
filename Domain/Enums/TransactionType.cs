namespace CushyPay.Domain.Enums;

public enum TransactionType
{
    InternalTransfer = 1,    // Transfer between CushyPay users
    ExternalTransfer = 2,    // Transfer to external bank account
    Deposit = 3,             // Deposit via credit card (mock)
    Withdrawal = 4          // Withdraw to bank account
}

