using CushyPay.Domain.Enums;

namespace CushyPay.Application.Common.DTOs;

public class WalletDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Iban { get; set; } = string.Empty;
    public Currency Currency { get; set; }
    public decimal Balance { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

