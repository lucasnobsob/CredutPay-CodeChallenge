
using CredutPay.Domain.Events;
using MediatR;

namespace CredutPay.Domain.EventHandlers
{
    public class TransactionEventHandler :
        INotificationHandler<TransactionRegisteredEvent>
    {
        public Task Handle(TransactionRegisteredEvent notification, CancellationToken cancellationToken)
        {
            // Send some greetings e-mail

            return Task.CompletedTask;
        }
    }
}
