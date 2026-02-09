using MediatR;
using Microsoft.EntityFrameworkCore;
using CushyPay.Application.Common.DTOs;
using CushyPay.Application.Common.Responses;
using CushyPay.Domain.Enums;
using CushyPay.Infrastructure.Data;

namespace CushyPay.Application.Features.Transactions.Queries.GetTransactionHistory;

public class GetTransactionHistoryQuery : IRequest<Result<List<TransactionDto>>>
{
    public int WalletId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public TransactionType? Type { get; set; }
    public TransactionStatus? Status { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
}

public class GetTransactionHistoryQueryHandler : IRequestHandler<GetTransactionHistoryQuery, Result<List<TransactionDto>>>
{
    private readonly AppDbContext _context;

    public GetTransactionHistoryQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<TransactionDto>>> Handle(GetTransactionHistoryQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Transactions
            .Where(t => t.FromWalletId == request.WalletId || t.ToWalletId == request.WalletId);

        if (request.FromDate.HasValue)
            query = query.Where(t => t.CreatedAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(t => t.CreatedAt <= request.ToDate.Value);

        if (request.Type.HasValue)
            query = query.Where(t => t.Type == request.Type.Value);

        if (request.Status.HasValue)
            query = query.Where(t => t.Status == request.Status.Value);

        if (request.MinAmount.HasValue)
            query = query.Where(t => t.Amount >= request.MinAmount.Value);

        if (request.MaxAmount.HasValue)
            query = query.Where(t => t.Amount <= request.MaxAmount.Value);

        var transactions = await query
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                FromWalletId = t.FromWalletId,
                ToWalletId = t.ToWalletId,
                ExternalAccountNumber = t.ExternalAccountNumber,
                ExternalBankName = t.ExternalBankName,
                Amount = t.Amount,
                Currency = t.Currency,
                Type = t.Type,
                Status = t.Status,
                Description = t.Description,
                ReferenceNumber = t.ReferenceNumber!,
                ProcessedAt = t.ProcessedAt,
                FailureReason = t.FailureReason,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<List<TransactionDto>>.Success(transactions);
    }
}

