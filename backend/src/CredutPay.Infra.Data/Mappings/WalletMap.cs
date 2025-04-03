using CredutPay.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CredutPay.Infra.Data.Mappings
{
    public class WalletMap : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(w => w.Balance)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(w => w.Name)
                   .HasColumnType("varchar(50)");

            builder.HasOne(w => w.User)
                   .WithMany(u => u.Wallets)
                   .HasForeignKey(w => w.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(w => w.SourceTransactions)
                   .WithOne(t => t.SourceWallet)
                   .HasForeignKey(t => t.SourceWalletId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(w => w.DestinationTransactions)
                   .WithOne(t => t.DestinationWallet)
                   .HasForeignKey(t => t.DestinationWalletId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Wallet");
        }
    }
}
