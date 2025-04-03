
using CredutPay.Domain.Models;

namespace CredutPay.Domain.Specifications
{
    public class TransactionFilterPaginatedSpecification : BaseSpecification<Transaction>
    {
        public TransactionFilterPaginatedSpecification(int skip, int take, Guid walletId)
            : base(i => i.SourceWalletId == walletId)
        {
            ApplyPaging(skip, take);
        }
    }
}
