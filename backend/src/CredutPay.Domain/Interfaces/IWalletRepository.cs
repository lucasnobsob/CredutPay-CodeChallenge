using CredutPay.Domain.Models;

namespace CredutPay.Domain.Interfaces
{
    public interface IWalletRepository : IRepository<Wallet>
    {
        Task<Wallet?> GetByUserId(Guid Id);
    }
}
