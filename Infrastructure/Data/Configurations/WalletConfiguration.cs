using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CushyPay.Domain.Entities;

namespace CushyPay.Infrastructure.Data.Configurations;

public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("Wallets");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.UserId)
            .IsRequired();

        builder.Property(w => w.Iban)
            .IsRequired()
            .HasMaxLength(34);

        builder.Property(w => w.Currency)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(w => w.Balance)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(w => w.CreatedAt)
            .IsRequired();

        builder.Property(w => w.UpdatedAt);

        builder.Property(w => w.RowVersion)
            .IsRowVersion()
            .IsRequired();

        builder.HasIndex(w => w.Iban)
            .IsUnique();

        builder.HasIndex(w => w.UserId);

    }
}

