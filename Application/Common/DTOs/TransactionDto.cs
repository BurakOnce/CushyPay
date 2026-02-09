using CushyPay.Domain.Enums;

namespace CushyPay.Application.Common.DTOs;

public class TransactionDto
{
    public int Id { get; set; }
    public int? FromWalletId { get; set; }
    public int? ToWalletId { get; set; }
    public string? ExternalAccountNumber { get; set; }
    public string? ExternalBankName { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public TransactionType Type { get; set; }
    public TransactionStatus Status { get; set; }
    public string? Description { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public DateTime? ProcessedAt { get; set; }
    public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; set; }
}

