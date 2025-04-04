using Bogus;
using CredutPay.Application.EventSourcedNormalizers;

namespace CredutPay.Tests.FakeData.Transaction
{
    public class TransactionHistoryFaker : Faker<TransactionHistoryData>
    {
        public TransactionHistoryFaker()
        {
            RuleFor(x => x.Id, y => Guid.NewGuid().ToString());
            RuleFor(x => x.Amount, y => y.Random.Decimal());
            RuleFor(x => x.When, y => y.Date.Between(DateTime.MinValue, DateTime.Now).ToString());
            RuleFor(x => x.Who, y => Guid.NewGuid().ToString());
        }
    }
}
