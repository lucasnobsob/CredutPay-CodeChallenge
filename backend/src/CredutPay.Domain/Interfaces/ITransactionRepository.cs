using CredutPay.Domain.Models;

namespace CredutPay.Domain.Interfaces
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetByWalletId(Guid Id);
    }
}
