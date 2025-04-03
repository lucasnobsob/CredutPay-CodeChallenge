using AutoMapper;
using CredutPay.Application.EventSourcedNormalizers;
using CredutPay.Application.Interfaces;
using CredutPay.Application.ViewModels;
using CredutPay.Domain.Commands;
using CredutPay.Domain.Core.Interfaces;
using CredutPay.Domain.Interfaces;
using CredutPay.Infra.Data.Repository.EventSourcing;

namespace CredutPay.Application.Services
{
    public class WalletAppService : IWalletAppService
    {
        private readonly IMapper _mapper;
        private readonly IWalletRepository _walletRepository;
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly IMediatorHandler Bus;

        public WalletAppService(IMapper mapper, 
                                IWalletRepository walletRepository, 
                                IEventStoreRepository eventStoreRepository, 
                                IMediatorHandler bus)
        {
            _mapper = mapper;
            _walletRepository = walletRepository;
            _eventStoreRepository = eventStoreRepository;
            Bus = bus;
        }

        public async Task<IEnumerable<WalletViewModel>> GetAll()
        {
            var wallet = await _walletRepository.GetAll();
            return _mapper.Map<List<WalletViewModel>>(wallet);
        }

        public async Task<IList<WalletHistoryData>> GetAllHistory(Guid id)
        {
            var storedEvents = await _eventStoreRepository.All(id);
            return WalletHistory.ToJavaScriptCustomerHistory(storedEvents);
        }

        public async Task<WalletViewModel> GetById(Guid id)
        {
            var wallet = await _walletRepository.GetById(id);
            return _mapper.Map<WalletViewModel>(wallet);
        }

        public async Task<WalletViewModel> GetByUserId(Guid Id)
        {
            var wallet = await _walletRepository.GetByUserId(Id);
            return _mapper.Map<WalletViewModel>(wallet);
        }

        public async Task Register(CreateWalletViewModel walletViewModel)
        {
            var registerCommand = _mapper.Map<RegisterNewWalletCommand>(walletViewModel);
            await Bus.SendCommand(registerCommand);
        }

        public async Task Remove(Guid id)
        {
            var removeCommand = new RemoveWalletCommand(id);
            await Bus.SendCommand(removeCommand);
        }
    }
}
