using CredutPay.Domain.Validations;

namespace CredutPay.Domain.Commands
{
    public class RegisterNewWalletCommand : WalletCommand
    {
        public RegisterNewWalletCommand(string name, Guid userId)
        {
            Name = name;
            UserId = userId;
            Balance = 0;
        }

        public override bool IsValid()
        {
            ValidationResult = new RegisterNewWalletValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
