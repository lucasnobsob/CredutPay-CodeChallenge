using CredutPay.Domain.Validations;

namespace CredutPay.Domain.Commands
{
    public class RemoveWalletCommand : WalletCommand
    {
        public RemoveWalletCommand(Guid id)
        {
            Id = id;
        }

        public override bool IsValid()
        {
            ValidationResult = new RemoveWalletValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
