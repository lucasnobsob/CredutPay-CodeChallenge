using CredutPay.Domain.Commands;

namespace CredutPay.Domain.Validations
{
    public class RegisterNewTransactionValidation : TransactionValidation<RegisterNewTransactionCommand>
    {
        public RegisterNewTransactionValidation()
        {
            ValidateAmount();
            ValidateDescription();
            ValidateWallets();
        }
    }
}
