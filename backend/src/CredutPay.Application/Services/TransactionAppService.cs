using AutoMapper;
using CredutPay.Application.EventSourcedNormalizers;
using CredutPay.Application.Interfaces;
using CredutPay.Application.ViewModels;
using CredutPay.Domain.Commands;
using CredutPay.Domain.Core.Interfaces;
using CredutPay.Domain.Interfaces;
using CredutPay.Domain.Specifications;
using CredutPay.Infra.Data.Repository.EventSourcing;

namespace CredutPay.Application.Services
{
    public class TransactionAppService : ITransactionAppService
    {
        private readonly IMapper _mapper;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IEventStoreRepository _eventStoreSqlRepository;
        private readonly IMediatorHandler Bus;

        public TransactionAppService(IMapper mapper, 
                                     ITransactionRepository transactionRepository, 
                                     IEventStoreRepository eventStoreSqlRepository, 
                                     IMediatorHandler bus)
        {
            _mapper = mapper;
            _transactionRepository = transactionRepository;
            _eventStoreSqlRepository = eventStoreSqlRepository;
            Bus = bus;
        }

        public async Task<IEnumerable<TransactionViewModel>> GetAll()
        {
            var transactions = await _transactionRepository.GetAll();
            return _mapper.Map<List<TransactionViewModel>>(transactions);
        }

        public async Task<PaginatedSuccessResult<TransactionViewModel>> GetAll(int skip, int take, Guid walletId)
        {
            var spec = new TransactionFilterPaginatedSpecification(skip, take, walletId);
            var transactions = await _transactionRepository.GetAll(spec);
            var mappedTransactions = _mapper.Map<List<TransactionViewModel>>(transactions);
            var totalCount = await _transactionRepository.GetTotalCount(walletId);

            return new PaginatedSuccessResult<TransactionViewModel>(mappedTransactions, totalCount);
        }

        public async Task<IList<TransactionHistoryData>> GetAllHistory(Guid id)
        {
            var storedEvents = await _eventStoreSqlRepository.All(id);
            return TransactionHistory.ToJavaScriptCustomerHistory(storedEvents);
        }

        public async Task<IEnumerable<TransactionViewModel>> GetByWalletId(Guid id)
        {
            var transactions = await _transactionRepository.GetByWalletId(id);
            return _mapper.Map<List<TransactionViewModel>>(transactions);
        }

        public async Task Register(CreateTransactionViewModel transactionViewModel)
        {
            var registerCommand = _mapper.Map<RegisterNewTransactionCommand>(transactionViewModel);
            await Bus.SendCommand(registerCommand);
        }
    }
}
