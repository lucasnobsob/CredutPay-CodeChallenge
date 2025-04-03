using Bogus;
using CredutPay.Application.ViewModels;

namespace CredutPay.Tests.FakeData.Wallet
{
    public class WalletFaker : Faker<Domain.Models.Wallet>
    {
        public WalletFaker()
        {
            RuleFor(x => x.Id, y => Guid.NewGuid());
            RuleFor(x => x.Name, y => y.Name.FullName());
            RuleFor(x => x.Balance, y => y.Random.Decimal(1, decimal.MaxValue));
            RuleFor(x => x.UserId, y => Guid.NewGuid().ToString());
        }
    }

    public class WalletViewModelFaker : Faker<WalletViewModel>
    {
        public WalletViewModelFaker()
        {
            RuleFor(x => x.Id, y => Guid.NewGuid());
            RuleFor(x => x.Name, y => y.Name.FullName());
            RuleFor(x => x.Balance, y => y.Random.Decimal(1, decimal.MaxValue));
            RuleFor(x => x.UserId, y => Guid.NewGuid());
        }
    }
}
