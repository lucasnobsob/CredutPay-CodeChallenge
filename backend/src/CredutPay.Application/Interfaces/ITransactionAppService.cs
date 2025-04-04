using CredutPay.Application.EventSourcedNormalizers;
using CredutPay.Application.ViewModels;

namespace CredutPay.Application.Interfaces
{
    public interface ITransactionAppService
    {
        Task Register(CreateTransactionViewModel transactionViewModel);
        Task<IEnumerable<TransactionViewModel>> GetAll();
        Task<PaginatedSuccessResult<TransactionViewModel>> GetAll(int skip, int take, Guid walletId);
        Task<IEnumerable<TransactionViewModel>> GetByWalletId(Guid id);
        Task<IList<TransactionHistoryData>> GetAllHistory(Guid id);
    }
}
