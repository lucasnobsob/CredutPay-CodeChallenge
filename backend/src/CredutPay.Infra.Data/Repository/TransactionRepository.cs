using CredutPay.Domain.Interfaces;
using CredutPay.Domain.Models;
using CredutPay.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CredutPay.Infra.Data.Repository
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(ApplicationDbContext context) 
            : base(context)
        {
        }

        public async Task<IEnumerable<Transaction>> GetByWalletId(Guid Id)
        {
            return await DbSet.AsNoTracking()
                .Include(w => w.SourceWallet)
                .Include(w => w.DestinationWallet)
                .Where(w => w.SourceWalletId == Id)
                .OrderByDescending(w => w.Date)
                .ToListAsync();
        }

        public async Task<int> GetTotalCount(Guid WalletId)
        {
            return await DbSet.AsNoTracking().Where(x => x.SourceWalletId == WalletId).CountAsync();
        }
    }
}
