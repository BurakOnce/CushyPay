using FluentValidation;

namespace CushyPay.Application.Features.Transactions.Commands.Deposit;

public class DepositCommandValidator : AbstractValidator<DepositCommand>
{
    public DepositCommandValidator()
    {
        RuleFor(x => x.ToWalletId)
            .GreaterThan(0).WithMessage("Wallet ID must be greater than zero.");

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

