using CredutPay.Application.EventSourcedNormalizers;
using CredutPay.Application.ViewModels;

namespace CredutPay.Application.Interfaces
{
    public interface IWalletAppService
    {
        Task Register(CreateWalletViewModel walletViewModel);
        Task<IEnumerable<WalletViewModel>> GetAllByUserId(Guid userId);
        Task<WalletViewModel> GetById(Guid id);
        Task Remove(Guid id);
        Task<IList<WalletHistoryData>> GetAllHistory(Guid id);
        Task<IEnumerable<WalletViewModel>> GetAll(Guid userId);
    }
}
