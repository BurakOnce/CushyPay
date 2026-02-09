using MediatR;
using Microsoft.EntityFrameworkCore;
using CushyPay.Application.Common.DTOs;
using CushyPay.Application.Common.Interfaces;
using CushyPay.Application.Common.Responses;
using CushyPay.Domain.Entities;
using CushyPay.Domain.Enums;
using CushyPay.Domain.Exceptions;
using CushyPay.Infrastructure.Data;

namespace CushyPay.Application.Features.Transactions.Commands.InternalTransfer;

public class InternalTransferCommand : IRequest<Result<TransactionDto>>
{
    public int FromWalletId { get; set; }
    public int ToWalletId { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public string? Description { get; set; }
}

public class InternalTransferCommandHandler : IRequestHandler<InternalTransferCommand, Result<TransactionDto>>
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public InternalTransferCommandHandler(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TransactionDto>> Handle(InternalTransferCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var fromWallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.Id == request.FromWalletId && w.IsActive, cancellationToken);

            var toWallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.Id == request.ToWalletId && w.IsActive, cancellationToken);

            if (fromWallet == null)
                return Result<TransactionDto>.Failure("Source wallet not found.");

            if (toWallet == null)
                return Result<TransactionDto>.Failure("Destination wallet not found.");

            if (fromWallet.Currency != request.Currency || toWallet.Currency != request.Currency)
                return Result<TransactionDto>.Failure("Currency mismatch between wallets.");

            if (!fromWallet.HasSufficientBalance(request.Amount))
                return Result<TransactionDto>.Failure("Insufficient balance.");

            var transaction = Transaction.CreateInternalTransfer(
                request.FromWalletId,
                request.ToWalletId,
                request.Amount,
                request.Currency,
                request.Description);

            await _context.Transactions.AddAsync(transaction, cancellationToken);

            fromWallet.Debit(request.Amount);
            toWallet.Credit(request.Amount);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            transaction.MarkAsCompleted();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var transactionDto = new TransactionDto
            {
                Id = transaction.Id,
                FromWalletId = transaction.FromWalletId,
                ToWalletId = transaction.ToWalletId,
                Amount = transaction.Amount,
                Currency = transaction.Currency,
                Type = transaction.Type,
                Status = transaction.Status,
                Description = transaction.Description,
                ReferenceNumber = transaction.ReferenceNumber!,
                ProcessedAt = transaction.ProcessedAt,
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
            return Result<TransactionDto>.Failure($"Transaction failed: {ex.Message}");
        }
    }
}

