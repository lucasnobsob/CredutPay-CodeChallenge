using CredutPay.Domain.Core.Events;

namespace CredutPay.Domain.Events
{
    public class WalletRegisteredEvent : Event
    {
        public WalletRegisteredEvent(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
