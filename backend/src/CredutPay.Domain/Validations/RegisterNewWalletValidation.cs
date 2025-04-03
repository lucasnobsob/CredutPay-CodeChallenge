using CredutPay.Domain.Commands;

namespace CredutPay.Domain.Validations
{
    public class RegisterNewWalletValidation : WalletValidation<RegisterNewWalletCommand>
    {
        public RegisterNewWalletValidation()
        {
            ValidateName();
        }
    }
}
