using MediatR;
using Microsoft.EntityFrameworkCore;
using CushyPay.Application.Common.DTOs;
using CushyPay.Application.Common.Responses;
using CushyPay.Infrastructure.Data;

namespace CushyPay.Application.Features.Wallets.Queries.GetUserWallets;

public class GetUserWalletsQuery : IRequest<Result<List<WalletDto>>>
{
    public int UserId { get; set; }
}

public class GetUserWalletsQueryHandler : IRequestHandler<GetUserWalletsQuery, Result<List<WalletDto>>>
{
    private readonly AppDbContext _context;

    public GetUserWalletsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<WalletDto>>> Handle(GetUserWalletsQuery request, CancellationToken cancellationToken)
    {
        var wallets = await _context.Wallets
            .Where(w => w.UserId == request.UserId && w.IsActive)
            .OrderBy(w => w.CreatedAt)
            .Select(w => new WalletDto
            {
                Id = w.Id,
                UserId = w.UserId,
                Iban = w.Iban,
                Currency = w.Currency,
                Balance = w.Balance,
                Name = w.Name,
                IsActive = w.IsActive,
                CreatedAt = w.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<List<WalletDto>>.Success(wallets);
    }
}

