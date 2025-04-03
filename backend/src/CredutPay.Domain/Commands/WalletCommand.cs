using CredutPay.Domain.Core.Commands;

namespace CredutPay.Domain.Commands
{
    public abstract class WalletCommand : Command
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; } = 0m;
        public Guid UserId { get; set; }
    }
}
