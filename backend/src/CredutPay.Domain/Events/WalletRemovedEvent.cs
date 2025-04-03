using CredutPay.Domain.Core.Events;

namespace CredutPay.Domain.Events
{
    public class WalletRemovedEvent : Event
    {
        public WalletRemovedEvent(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}
