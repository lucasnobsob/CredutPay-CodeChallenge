
using CredutPay.Domain.Models;

namespace CredutPay.Domain.Specifications
{
    public class TransactionFilterPaginatedSpecification : BaseSpecification<Transaction>
    {
        public TransactionFilterPaginatedSpecification(int skip, int take, Guid walletId)
            : base(i => i.SourceWalletId == walletId)
        {
            AddInclude(x => x.SourceWallet);
            AddInclude(x => x.DestinationWallet);
            ApplyPaging(skip, take);
            ApplyOrderByDescending(x => x.Date);
        }
    }
}
