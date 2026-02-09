using MediatR;
using Microsoft.EntityFrameworkCore;
using CushyPay.Application.Common.DTOs;
using CushyPay.Application.Common.Interfaces;
using CushyPay.Application.Common.Responses;
using CushyPay.Domain.Entities;
using CushyPay.Domain.Enums;
using CushyPay.Infrastructure.Data;

namespace CushyPay.Application.Features.Transactions.Commands.Deposit;

public class DepositCommand : IRequest<Result<TransactionDto>>
{
    public int ToWalletId { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public string? Description { get; set; }
}

public class DepositCommandHandler : IRequestHandler<DepositCommand, Result<TransactionDto>>
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public DepositCommandHandler(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TransactionDto>> Handle(DepositCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.Id == request.ToWalletId && w.IsActive, cancellationToken);

            if (wallet == null)
                return Result<TransactionDto>.Failure("Wallet not found.");

            if (wallet.Currency != request.Currency)
                return Result<TransactionDto>.Failure("Currency mismatch.");

            var transaction = Transaction.CreateDeposit(
                request.ToWalletId,
                request.Amount,
                request.Currency,
                request.Description);

            await _context.Transactions.AddAsync(transaction, cancellationToken);

            wallet.Credit(request.Amount);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            transaction.MarkAsCompleted();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var transactionDto = new TransactionDto
            {
                Id = transaction.Id,
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
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<TransactionDto>.Failure($"Deposit failed: {ex.Message}");
        }
    }
}

