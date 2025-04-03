using CredutPay.Domain.Models;
using CredutPay.Domain.Validations;

namespace CredutPay.Domain.Commands
{
    public class RegisterNewTransactionCommand : TransactionCommand
    {
        public RegisterNewTransactionCommand(decimal amount, string description, Guid sourceWalletId, Guid destinationWalletId)
        {
            Amount = amount;
            Description = description;
            Date = DateTime.Now;
            SourceWalletId = sourceWalletId;
            DestinationWalletId = destinationWalletId;
        }

        public override bool IsValid()
        {
            ValidationResult = new RegisterNewTransactionValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
