using CredutPay.Domain.Commands;
using FluentValidation;

namespace CredutPay.Domain.Validations
{
    public abstract class TransactionValidation<T> : AbstractValidator<T> where T : TransactionCommand
    {
        protected void ValidateDescription()
        {
            RuleFor(c => c.Description)
                .NotEmpty().WithMessage("Por favor, certifique-se de ter inserido a Descrição")
                .Length(2, 50).WithMessage("A descrição deve ter entre 2 e 255 caracteres");
        }

        protected void ValidateAmount()
        {
            RuleFor(c => c.Amount)
                .GreaterThan(0).WithMessage("A valor deve ser maior que zero");
        }

        protected void ValidateWallets()
        {
            RuleFor(c => c.SourceWalletId)
                .NotEqual(Guid.Empty)
                .WithMessage("A carteira origem deve ter um identificador válido");

            RuleFor(c => c.DestinationWalletId)
                .NotEqual(Guid.Empty)
                .WithMessage("A carteira destino deve ter um identificador válido");

            RuleFor(c => c)
                .Must(c => c.SourceWalletId != c.DestinationWalletId)
                .WithMessage("A carteira destino deve ser diferente da carteira origem");
        }
    }
}
