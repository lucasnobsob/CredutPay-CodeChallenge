using CredutPay.Domain.Core.Models;

namespace CredutPay.Domain.Models
{
    public class Transaction : Entity
    {
        public Transaction(Guid id, decimal amount, TransactionType type, string description, DateTime date, Guid sourceWalletId, Guid destinationWalletId)
        {
            Id = id;
            Amount = amount;
            Description = description;
            Type = type;
            Date = date;
            SourceWalletId = sourceWalletId;
            DestinationWalletId = destinationWalletId;
        }

        public Transaction(decimal amount, TransactionType type, string description, DateTime date, Guid sourceWalletId, Guid destinationWalletId)
        {
            Amount = amount;
            Description = description;
            Type = type;
            Date = date;
            SourceWalletId = sourceWalletId;
            DestinationWalletId = destinationWalletId;
        }

        public Transaction() { }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        public TransactionType Type { get; set; }

        // Foreign Keys
        public Guid SourceWalletId { get; set; }
        public Guid DestinationWalletId { get; set; }

        public Wallet SourceWallet { get; set; }
        public Wallet DestinationWallet { get; set; }
    }

    public enum TransactionType {
        Credit,
        Debit
    }
}
