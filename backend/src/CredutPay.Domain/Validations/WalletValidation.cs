using CredutPay.Domain.Commands;
using FluentValidation;

namespace CredutPay.Domain.Validations
{
    public abstract class WalletValidation<T> : AbstractValidator<T> where T : WalletCommand
    {
        protected void ValidateId()
        {
            RuleFor(c => c.Id)
                .NotEqual(Guid.Empty)
                .WithMessage("A carteira deve ter um identificador válido");
        }

        protected void ValidateName()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Por favor, certifique-se de ter inserido o Nome")
                .Length(2, 50).WithMessage("O nome deve ter entre 2 e 50 caracteres");
        }
    }
}
