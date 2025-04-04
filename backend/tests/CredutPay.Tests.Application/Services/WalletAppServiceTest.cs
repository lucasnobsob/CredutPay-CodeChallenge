using AutoMapper;
using CredutPay.Application.Interfaces;
using CredutPay.Application.Services;
using CredutPay.Application.ViewModels;
using CredutPay.Domain.Commands;
using CredutPay.Domain.Core.Events;
using CredutPay.Domain.Core.Interfaces;
using CredutPay.Domain.Interfaces;
using CredutPay.Domain.Models;
using CredutPay.Domain.Specifications;
using CredutPay.Infra.Data.Repository.EventSourcing;
using CredutPay.Tests.FakeData.Transaction;
using CredutPay.Tests.FakeData.Wallet;
using Moq;
using Xunit;

namespace CredutPay.Tests.Application.Services
{
    public class WalletAppServiceTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IWalletRepository> _walletRepositoryMock;
        private readonly Mock<IMediatorHandler> _mediatorHandlerMock;
        private readonly Mock<IEventStoreRepository> _eventStoreRepositoryMock;
        private readonly WalletAppService _walletAppService;

        public WalletAppServiceTest()
        {
            _mediatorHandlerMock = new Mock<IMediatorHandler>();
            _eventStoreRepositoryMock = new Mock<IEventStoreRepository>();
            _mapperMock = new Mock<IMapper>();
            _walletRepositoryMock = new Mock<IWalletRepository>();

            MockWalletMapping();
            _walletAppService = new WalletAppService(
                _mapperMock.Object,
                _walletRepositoryMock.Object,
                _eventStoreRepositoryMock.Object,
                _mediatorHandlerMock.Object
            );
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllWallets()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var wallets = new WalletFaker().Generate(10);

            _walletRepositoryMock.Setup(repo => repo.GetAll())
                .ReturnsAsync(wallets);

            // Act
            var result = await _walletAppService.GetAll(userId);

            // Assert
            Assert.Equal(10, result.Count());
        }

        [Fact]
        public async Task GetAllByUserId_ShouldReturnAllWalletsFromUser()
        {
            // Arrange
            var walletId = Guid.NewGuid();
            var wallets = new WalletFaker().Generate(10);

            _walletRepositoryMock.Setup(repo => repo.GetAllByUserId(walletId))
                .ReturnsAsync(wallets);

            // Act
            var result = await _walletAppService.GetAllByUserId(walletId);

            // Assert
            Assert.Equal(10, result.Count());
        }

        [Fact]
        public async Task GetById_ShouldReturnWalletFromId()
        {
            // Arrange
            var wallet = new WalletFaker().Generate();

            _walletRepositoryMock.Setup(repo => repo.GetById(wallet.Id))
                .ReturnsAsync(wallet);

            // Act
            var result = await _walletAppService.GetById(wallet.Id);

            // Assert
            Assert.Equal(result.Id, wallet.Id);
        }

        [Fact]
        public async Task Register_ShouldSendRegisterCommand()
        {
            // Arrange
            var register = new CreateWalletViewModel();
            var registerCommand = new RegisterNewWalletCommand("", Guid.NewGuid());
            _mapperMock.Setup(mapper => mapper.Map<RegisterNewWalletCommand>(register))
                .Returns(registerCommand);


            // Act
            await _walletAppService.Register(register);

            // Assert
            _mediatorHandlerMock.Verify(mediator => mediator.SendCommand(registerCommand), Times.Once);
        }

        [Fact]
        public async Task GetAllWalletHistory_ShouldReturnWalletHistory()
        {
            // Arrange
            var aggregateId = Guid.NewGuid();
            var storedEventList = new List<StoredEvent> { new StoredEvent { } };

            _eventStoreRepositoryMock.Setup(repo => repo.All(aggregateId))
                .ReturnsAsync(storedEventList);

            // Act
            var result = await _walletAppService.GetAllHistory(aggregateId);

            // Assert
            Assert.Equal(1, result.Count);
        }


        private void MockWalletMapping()
        {
            _mapperMock.Setup(m => m.Map<WalletViewModel>(It.IsAny<Wallet>()))
                .Returns((Wallet source) => new WalletViewModel
                {
                    Id = source.Id,
                    Balance = source.Balance,
                    Name = source.Name,
                    IsDeleted = source.IsDeleted
                });

            _mapperMock.Setup(m => m.Map<List<WalletViewModel>>(It.IsAny<List<Wallet>>()))
                .Returns((List<Wallet> source) => source.Select(e => new WalletViewModel
                {
                    Id = e.Id,
                    Balance = e.Balance,
                    Name = e.Name,
                    IsDeleted = e.IsDeleted
                }).ToList());

            _mapperMock.Setup(mapper => mapper.ConfigurationProvider)
                .Returns(new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Wallet, WalletViewModel>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                        .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => Guid.Parse(src.UserId)))
                        .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                        .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted))
                        .ReverseMap();
                }));
        }
    }
}
