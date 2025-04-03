using CredutPay.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CredutPay.Infra.Data.Mappings
{
    public class TransactionMap : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Date)
                   .IsRequired()
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(t => t.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(t => t.Description)
                   .HasColumnType("varchar(255)")
                   .IsRequired(false);

            builder.HasOne(t => t.SourceWallet)
                   .WithMany(t => t.SourceTransactions)
                   .HasForeignKey(t => t.SourceWalletId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.DestinationWallet)
                   .WithMany(t => t.DestinationTransactions)
                   .HasForeignKey(t => t.DestinationWalletId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Transaction");
        }
    }
}
