using System.Text.Json.Serialization;

namespace CredutPay.Application.ViewModels
{
    public class TransactionViewModel
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }

        [JsonIgnore]
        public TransactionType Type { get; set; }
        public string TransactionType { get; set; } = "";
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;

        public WalletViewModel SourceWallet { get; set; } = new WalletViewModel();
        public WalletViewModel DestinationWallet { get; set; } = new WalletViewModel();
    }

    public enum TransactionType
    {
        Credit,
        Debit
    }
}
