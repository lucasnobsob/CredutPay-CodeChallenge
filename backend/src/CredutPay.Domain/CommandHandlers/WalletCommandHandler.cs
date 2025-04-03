using CredutPay.Domain.Commands;
using CredutPay.Domain.Core.Interfaces;
using CredutPay.Domain.Core.Notifications;
using CredutPay.Domain.Events;
using CredutPay.Domain.Interfaces;
using CredutPay.Domain.Models;
using MediatR;

namespace CredutPay.Domain.CommandHandlers
{
    public class WalletCommandHandler : CommandHandler,
        IRequestHandler<RegisterNewWalletCommand, bool>,
        IRequestHandler<RemoveWalletCommand, bool>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IMediatorHandler Bus;

        public WalletCommandHandler(IWalletRepository walletRepository,
                                    IUnitOfWork uow,
                                    IMediatorHandler bus,
                                    INotificationHandler<DomainNotification> notifications) : 
            base(uow, bus, notifications)
        {
            _walletRepository = walletRepository;
            Bus = bus;
        }

        public async Task<bool> Handle(RegisterNewWalletCommand message, CancellationToken cancellationToken)
        {
            if (!message.IsValid())
            {
                NotifyValidationErrors(message);
                return await Task.FromResult(false);
            }
            var wallet = new Wallet(message.Name, message.UserId.ToString());
            await _walletRepository.Add(wallet);

            if (Commit())
            {
                await Bus.RaiseEvent(new WalletRegisteredEvent(message.Name));
            }
            return await Task.FromResult(true);
        }

        public async Task<bool> Handle(RemoveWalletCommand message, CancellationToken cancellationToken)
        {
            if (!message.IsValid())
            {
                NotifyValidationErrors(message);
                return await Task.FromResult(false);
            }

            await _walletRepository.Remove(message.Id);

            if (Commit())
            {
                await Bus.RaiseEvent(new WalletRemovedEvent(message.Id));
            }

            return await Task.FromResult(true);
        }

        public void Dispose()
        {
            _walletRepository.Dispose();
        }
    }
}
