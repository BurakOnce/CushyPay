using FluentValidation;

namespace CushyPay.Application.Features.Transactions.Commands.ExternalTransfer;

public class ExternalTransferCommandValidator : AbstractValidator<ExternalTransferCommand>
{
    public ExternalTransferCommandValidator()
    {
        RuleFor(x => x.FromWalletId)
            .GreaterThan(0).WithMessage("Wallet ID must be greater than zero.");

        RuleFor(x => x.ExternalAccountNumber)
            .NotEmpty().WithMessage("External account number is required.")
            .MaximumLength(50).WithMessage("Account number must not exceed 50 characters.");

        RuleFor(x => x.ExternalBankName)
            .NotEmpty().WithMessage("Bank name is required.")
            .MaximumLength(100).WithMessage("Bank name must not exceed 100 characters.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.")
            .LessThanOrEqualTo(1000000).WithMessage("Amount cannot exceed 1,000,000.");

        RuleFor(x => x.Currency)
            .IsInEnum().WithMessage("Invalid currency.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

