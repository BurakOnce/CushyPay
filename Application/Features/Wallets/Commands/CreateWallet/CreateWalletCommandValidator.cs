using FluentValidation;

namespace CushyPay.Application.Features.Wallets.Commands.CreateWallet;

public class CreateWalletCommandValidator : AbstractValidator<CreateWalletCommand>
{
    public CreateWalletCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID must be greater than zero.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Wallet name is required.")
            .MaximumLength(100).WithMessage("Wallet name must not exceed 100 characters.");

        RuleFor(x => x.Currency)
            .IsInEnum().WithMessage("Invalid currency.");
    }
}

