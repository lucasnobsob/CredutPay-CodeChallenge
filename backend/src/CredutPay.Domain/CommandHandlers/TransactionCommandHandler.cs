using CredutPay.Domain.Commands;
using CredutPay.Domain.Core.Interfaces;
using CredutPay.Domain.Core.Notifications;
using CredutPay.Domain.Events;
using CredutPay.Domain.Interfaces;
using CredutPay.Domain.Models;
using MediatR;

namespace CredutPay.Domain.CommandHandlers
{
    public class TransactionCommandHandler : CommandHandler,
        IRequestHandler<RegisterNewTransactionCommand, bool>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IMediatorHandler Bus;

        public TransactionCommandHandler(INotificationHandler<DomainNotification> notifications,
                                         ITransactionRepository transactionRepository,
                                         IWalletRepository walletRepository,
                                         IMediatorHandler bus,
                                         IUnitOfWork uow) : base(uow, bus, notifications)
        {
            _transactionRepository = transactionRepository;
            _walletRepository = walletRepository;
            Bus = bus;
        }

        public async Task<bool> Handle(RegisterNewTransactionCommand message, CancellationToken cancellationToken)
        {
            if (!message.IsValid())
            {
                NotifyValidationErrors(message);
                return await Task.FromResult(false);
            }

            var sourceWallet = await _walletRepository.GetById(message.SourceWalletId);
            var destinationWallet = await _walletRepository.GetById(message.DestinationWalletId);

            if (sourceWallet is null || destinationWallet is null)
            {
                await Bus.RaiseEvent(new DomainNotification(message.MessageType, "Carteira(s) não encontrada(s)."));
                return false;
            }

            if (sourceWallet.Balance < message.Amount)
            {
                await Bus.RaiseEvent(new DomainNotification(message.MessageType, "Saldo insuficiente para realizar a transação."));
                return await Task.FromResult(false);
            }

            DebitToWallet(message.Amount, sourceWallet);
            CreditToWallet(message.Amount, destinationWallet);

            var transaction1 = new Transaction(
                message.Amount,
                TransactionType.Debit,
                message.Description,
                message.Date,
                message.SourceWalletId,
                message.DestinationWalletId
            );

            var transaction2 = new Transaction(
                message.Amount,
                TransactionType.Credit,
                message.Description,
                message.Date,
                message.DestinationWalletId,
                message.SourceWalletId
            );

            await _transactionRepository.Add(transaction1);
            await _transactionRepository.Add(transaction2);
            _walletRepository.Update(sourceWallet);
            _walletRepository.Update(destinationWallet);

            if (Commit())
            {
                await Bus.RaiseEvent(new TransactionRegisteredEvent(message.Amount, message.Description, message.Date));
            }
            return await Task.FromResult(true);
        }

        private void CreditToWallet(decimal amount, Wallet wallet)
        {
            wallet.Balance += amount;
        }

        private void DebitToWallet(decimal amount, Wallet wallet)
        {
            wallet.Balance -= amount;
        }

        public void Dispose()
        {
            _transactionRepository.Dispose();
        }
    }
}
