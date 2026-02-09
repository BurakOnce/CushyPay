using MediatR;
using Microsoft.EntityFrameworkCore;
using CushyPay.Application.Common.DTOs;
using CushyPay.Application.Common.Interfaces;
using CushyPay.Application.Common.Responses;
using CushyPay.Domain.Entities;
using CushyPay.Domain.Enums;
using CushyPay.Infrastructure.Data;

namespace CushyPay.Application.Features.Wallets.Commands.CreateWallet;

public class CreateWalletCommand : IRequest<Result<WalletDto>>
{
    public int UserId { get; set; }
    public Currency Currency { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CreateWalletCommandHandler : IRequestHandler<CreateWalletCommand, Result<WalletDto>>
{
    private readonly AppDbContext _context;

    public CreateWalletCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<WalletDto>> Handle(CreateWalletCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
            return Result<WalletDto>.Failure("User not found.");

        var wallet = Wallet.Create(request.UserId, request.Currency, request.Name);

        await _context.Wallets.AddAsync(wallet, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var walletDto = new WalletDto
        {
            Id = wallet.Id,
            UserId = wallet.UserId,
            Iban = wallet.Iban,
            Currency = wallet.Currency,
            Balance = wallet.Balance,
            Name = wallet.Name,
            IsActive = wallet.IsActive,
            CreatedAt = wallet.CreatedAt
        };

        return Result<WalletDto>.Success(walletDto);
    }
}

