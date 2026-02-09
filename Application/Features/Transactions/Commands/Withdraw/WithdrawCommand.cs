using MediatR;
using Microsoft.EntityFrameworkCore;
using CushyPay.Application.Common.DTOs;
using CushyPay.Application.Common.Interfaces;
using CushyPay.Application.Common.Responses;
using CushyPay.Domain.Entities;
using CushyPay.Domain.Enums;
using CushyPay.Infrastructure.Data;

namespace CushyPay.Application.Features.Transactions.Commands.Withdraw;

public class WithdrawCommand : IRequest<Result<TransactionDto>>
{
    public int FromWalletId { get; set; }
    public string ExternalAccountNumber { get; set; } = string.Empty;
    public string ExternalBankName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public string? Description { get; set; }
}

public class WithdrawCommandHandler : IRequestHandler<WithdrawCommand, Result<TransactionDto>>
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public WithdrawCommandHandler(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TransactionDto>> Handle(WithdrawCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.Id == request.FromWalletId && w.IsActive, cancellationToken);

            if (wallet == null)
                return Result<TransactionDto>.Failure("Wallet not found.");

            if (wallet.Currency != request.Currency)
                return Result<TransactionDto>.Failure("Currency mismatch.");

            if (!wallet.HasSufficientBalance(request.Amount))
                return Result<TransactionDto>.Failure("Insufficient balance.");

            var transaction = Transaction.CreateWithdrawal(
                request.FromWalletId,
                request.ExternalAccountNumber,
                request.ExternalBankName,
                request.Amount,
                request.Currency,
                request.Description);

            await _context.Transactions.AddAsync(transaction, cancellationToken);

            wallet.Debit(request.Amount);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);


            var transactionDto = new TransactionDto
            {
                Id = transaction.Id,
                FromWalletId = transaction.FromWalletId,
                ExternalAccountNumber = transaction.ExternalAccountNumber,
                ExternalBankName = transaction.ExternalBankName,
                Amount = transaction.Amount,
                Currency = transaction.Currency,
                Type = transaction.Type,
                Status = transaction.Status,
                Description = transaction.Description,
                ReferenceNumber = transaction.ReferenceNumber!,
                CreatedAt = transaction.CreatedAt
            };

            return Result<TransactionDto>.Success(transactionDto);
        }
        catch (DbUpdateConcurrencyException)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<TransactionDto>.Failure("Transaction failed due to concurrency conflict. Please try again.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<TransactionDto>.Failure($"Withdrawal failed: {ex.Message}");
        }
    }
}

