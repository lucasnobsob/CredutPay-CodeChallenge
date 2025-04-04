using AutoMapper;
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
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Numerics;
using Xunit;

namespace CredutPay.Tests.Application.Services
{
    public class TransactionAppServiceTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
        private readonly Mock<IMediatorHandler> _mediatorHandlerMock;
        private readonly Mock<IEventStoreRepository> _eventStoreRepositoryMock;
        private readonly TransactionAppService _transactionAppService;

        public TransactionAppServiceTest()
        {
            _mediatorHandlerMock = new Mock<IMediatorHandler>();
            _eventStoreRepositoryMock = new Mock<IEventStoreRepository>();
            _mapperMock = new Mock<IMapper>();
            _transactionRepositoryMock = new Mock<ITransactionRepository>();

            MockTransactionMapping();
            _transactionAppService = new TransactionAppService(
                _mapperMock.Object, 
                _transactionRepositoryMock.Object,
                _eventStoreRepositoryMock.Object,
                _mediatorHandlerMock.Object
            );
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllTransactions()
        {
            // Arrange
            var transactions = new TransactionFaker().Generate(10);

            _transactionRepositoryMock.Setup(repo => repo.GetAll())
                .ReturnsAsync(transactions);

            // Act
            var result = await _transactionAppService.GetAll();

            // Assert
            Assert.Equal(10, result.Count());
        }

        [Fact]
        public async Task GetAllPaginated_ShouldReturnAllPaginatedTransactions()
        {
            // Arrange
            int skip = 5, take = 5;
            var walletId = Guid.NewGuid();
            var spec = new TransactionFilterPaginatedSpecification(skip, take, walletId);
            var transactions = new TransactionFaker().Generate(10);

            _transactionRepositoryMock.Setup(repo => repo.GetAll(spec))
                .ReturnsAsync(transactions);
            _transactionRepositoryMock.Setup(repo => repo.GetTotalCount(walletId))
                .ReturnsAsync(10);

            // Act
            var result = await _transactionAppService.GetAll(skip, take, walletId);

            // Assert
            var okResult = Assert.IsType<PaginatedSuccessResult<TransactionViewModel>>(result);
            Assert.Equal(10, okResult.TotalCount);
        }

        [Fact]
        public async Task GetByWalletId_ShouldReturnAllTransactionsFromWallet()
        {
            // Arrange
            var walletId = Guid.NewGuid();
            var transactions = new TransactionFaker().Generate(10);

            _transactionRepositoryMock.Setup(repo => repo.GetByWalletId(walletId))
                .ReturnsAsync(transactions);

            // Act
            var result = await _transactionAppService.GetByWalletId(walletId);

            // Assert
            Assert.Equal(10, result.Count());
        }

        [Fact]
        public async Task Register_ShouldSendRegisterCommand()
        {
            // Arrange
            var register = new CreateTransactionViewModel();
            var registerCommand = new RegisterNewTransactionCommand(0, "", Guid.NewGuid(), Guid.NewGuid());
            _mapperMock.Setup(mapper => mapper.Map<RegisterNewTransactionCommand>(register))
                .Returns(registerCommand);


            // Act
            await _transactionAppService.Register(register);

            // Assert
            _mediatorHandlerMock.Verify(mediator => mediator.SendCommand(registerCommand), Times.Once);
        }

        [Fact]
        public async Task GetAllTransactionHistory_ShouldReturnTransactionHistory()
        {
            // Arrange
            var aggregateId = Guid.NewGuid();
            var storedEventList = new List<StoredEvent> { new StoredEvent { } };

            _eventStoreRepositoryMock.Setup(repo => repo.All(aggregateId))
                .ReturnsAsync(storedEventList);

            // Act
            var result = await _transactionAppService.GetAllHistory(aggregateId);

            // Assert
            Assert.Equal(1, result.Count);
        }

        private void MockTransactionMapping()
        {
            _mapperMock.Setup(m => m.Map<List<TransactionViewModel>>(It.IsAny<List<Transaction>>()))
                .Returns((List<Transaction> source) => source.Select(e => new TransactionViewModel
                {
                    Id = e.Id,
                    Amount = e.Amount,
                    Description = e.Description,
                    Date = e.Date
                }).ToList());

            _mapperMock.Setup(mapper => mapper.ConfigurationProvider)
                .Returns(new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Transaction, TransactionViewModel>()
                        .ForMember(x => x.TransactionType, y => y.MapFrom(z => z.Type.ToString()))
                        .ReverseMap();

                    cfg.CreateMap<IEnumerable<Transaction>, IEnumerable<TransactionViewModel>>().ReverseMap();

                    cfg.CreateMap<Transaction, CreateTransactionViewModel>().ReverseMap();
                }));
        }
    }
}
