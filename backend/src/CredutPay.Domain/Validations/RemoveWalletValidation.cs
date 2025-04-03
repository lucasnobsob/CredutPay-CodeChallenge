using CredutPay.Domain.Commands;

namespace CredutPay.Domain.Validations
{
    public class RemoveWalletValidation : WalletValidation<RemoveWalletCommand>
    {
        public RemoveWalletValidation()
        {
            ValidateId();
        }
    }
}
