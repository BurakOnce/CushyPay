using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CushyPay.Domain.Entities;

namespace CushyPay.Infrastructure.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.FromWalletId);
        builder.Property(t => t.ToWalletId);
        builder.Property(t => t.ExternalAccountNumber)
            .HasMaxLength(50);
        builder.Property(t => t.ExternalBankName)
            .HasMaxLength(100);

        builder.Property(t => t.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(t => t.Currency)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(t => t.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(t => t.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.ReferenceNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.ProcessedAt);
        builder.Property(t => t.FailureReason)
            .HasMaxLength(500);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt);

        builder.HasIndex(t => t.ReferenceNumber)
            .IsUnique();

        builder.HasIndex(t => t.FromWalletId);
        builder.HasIndex(t => t.ToWalletId);
        builder.HasIndex(t => t.Type);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.CreatedAt);

        builder.HasOne(t => t.FromWallet)
            .WithMany(w => w.OutgoingTransactions)
            .HasForeignKey(t => t.FromWalletId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasOne(t => t.ToWallet)
            .WithMany()
            .HasForeignKey(t => t.ToWalletId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}

