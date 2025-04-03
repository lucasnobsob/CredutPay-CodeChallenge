using Bogus;

namespace CredutPay.Tests.FakeData.Transaction
{
    public class TransactionFaker : Faker<Domain.Models.Transaction>
    {
        public TransactionFaker()
        {
            RuleFor(x => x.Id, y => Guid.NewGuid());
            RuleFor(x => x.Amount, y => y.Random.Decimal(1, decimal.MaxValue));
            RuleFor(x => x.Description, y => y.Lorem.Lines(1));
            RuleFor(x => x.Date, y => y.Date.Between(DateTime.MinValue, DateTime.Now));
            RuleFor(x => x.SourceWalletId, y => Guid.NewGuid());
            RuleFor(x => x.DestinationWalletId, y => Guid.NewGuid());
        }
    }
}
