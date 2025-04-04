using Bogus;
using CredutPay.Application.EventSourcedNormalizers;

namespace CredutPay.Tests.FakeData.Wallet
{
    public class WalletHistoryFaker : Faker<WalletHistoryData>
    {
        public WalletHistoryFaker()
        {
            RuleFor(x => x.Id, y => Guid.NewGuid().ToString());
            RuleFor(x => x.Name, y => y.Name.FullName());
            RuleFor(x => x.When, y => y.Date.Between(DateTime.MinValue, DateTime.Now).ToString());
            RuleFor(x => x.Who, y => Guid.NewGuid().ToString());
        }
    }
}
