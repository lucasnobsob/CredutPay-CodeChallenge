using CredutPay.Domain.Core.Commands;
using CredutPay.Domain.Models;

namespace CredutPay.Domain.Commands
{
    public abstract class TransactionCommand : Command
    {
        public decimal Amount { get; set; }
        public string Description { get; set; } = "";
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public Guid WalletId { get; set; }

        public Guid SourceWalletId { get; set; }
        public Guid DestinationWalletId { get; set; }
    }
}
