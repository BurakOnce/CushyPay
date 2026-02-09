using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using CushyPay.Application.Common.Interfaces;
using CushyPay.Domain.Entities;
using System.Text.Json;

namespace CushyPay.Infrastructure.Data;

public class AppDbContext : DbContext, IApplicationDbContext
{
    private int? _currentUserId;
    private string? _currentUserEmail;
    private string? _currentIpAddress;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public void SetCurrentUser(int? userId, string? userEmail, string? ipAddress = null)
    {
        _currentUserId = userId;
        _currentUserEmail = userEmail;
        _currentIpAddress = ipAddress;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Domain.Common.BaseEntity && 
                       (e.State == EntityState.Added || 
                        e.State == EntityState.Modified || 
                        e.State == EntityState.Deleted))
            .ToList();

        var auditLogs = new List<AuditLog>();

        foreach (var entry in entries)
        {
            var entity = (Domain.Common.BaseEntity)entry.Entity;
            var entityName = entry.Entity.GetType().Name;
            var entityId = entity.Id;

            string action;
            string? changes = null;

            switch (entry.State)
            {
                case EntityState.Added:
                    action = "Created";
                    break;
                case EntityState.Modified:
                    action = "Updated";
                    changes = GetChanges(entry);
                    break;
                case EntityState.Deleted:
                    action = "Deleted";
                    break;
                default:
                    continue;
            }

            var auditLog = AuditLog.Create(
                entityName,
                entityId,
                action,
                changes,
                _currentUserId,
                _currentUserEmail,
                _currentIpAddress);

            auditLogs.Add(auditLog);
        }

        if (auditLogs.Any())
        {
            await AuditLogs.AddRangeAsync(auditLogs, cancellationToken);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private static string? GetChanges(EntityEntry entry)
    {
        var changes = new Dictionary<string, object?>();

        foreach (var property in entry.Properties)
        {
            if (property.IsModified)
            {
                changes[property.Metadata.Name] = new
                {
                    OldValue = property.OriginalValue,
                    NewValue = property.CurrentValue
                };
            }
        }

        return changes.Any() ? JsonSerializer.Serialize(changes) : null;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}

