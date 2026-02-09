using Microsoft.EntityFrameworkCore;
using CushyPay.Domain.Entities;

namespace CushyPay.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Wallet> Wallets { get; }
    DbSet<Transaction> Transactions { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<RefreshToken> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

