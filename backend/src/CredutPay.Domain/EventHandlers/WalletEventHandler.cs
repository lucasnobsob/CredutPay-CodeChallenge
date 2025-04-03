using CredutPay.Domain.Events;
using MediatR;

namespace CredutPay.Domain.EventHandlers
{
    public class WalletEventHandler :
        INotificationHandler<WalletRegisteredEvent>,
        INotificationHandler<WalletRemovedEvent>
    {
        public Task Handle(WalletRegisteredEvent notification, CancellationToken cancellationToken)
        {
            // Send some greetings e-mail

            return Task.CompletedTask;
        }

        public Task Handle(WalletRemovedEvent notification, CancellationToken cancellationToken)
        {
            // Send some see you soon e-mail

            return Task.CompletedTask;
        }
    }
}
