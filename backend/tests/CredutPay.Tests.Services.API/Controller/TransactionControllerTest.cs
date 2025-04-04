using CredutPay.Application.EventSourcedNormalizers;
using CredutPay.Application.Interfaces;
using CredutPay.Application.ViewModels;
using CredutPay.Domain.Core.Interfaces;
using CredutPay.Domain.Core.Notifications;
using CredutPay.Services.API.Controllers;
using CredutPay.Tests.FakeData.Transaction;
using CredutPay.Tests.FakeData.Wallet;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CredutPay.Tests.Services.API.Controller
{
    public class TransactionControllerTest
    {
        private readonly Mock<DomainNotificationHandler> _notificationHandlerMock;
        private readonly Mock<ITransactionAppService> _transactionAppServiceMock;
        private readonly Mock<ILogger<TransactionController>> _loggerMock;
        private readonly Mock<IMediatorHandler> _mediatorHandlerMock;
        private readonly TransactionController _transactionController;

        public TransactionControllerTest()
        {
            _notificationHandlerMock = new Mock<DomainNotificationHandler>();
            _transactionAppServiceMock = new Mock<ITransactionAppService>();
            _loggerMock = new Mock<ILogger<TransactionController>>();
            _mediatorHandlerMock = new Mock<IMediatorHandler>();

            _transactionController = new TransactionController(
                _notificationHandlerMock.Object,
                _transactionAppServiceMock.Object,
                _loggerMock.Object,
                _mediatorHandlerMock.Object
            );
        }

        [Fact]
        public async Task GetByWalletId_ShouldReturnAllTransactionsFromWallet()
        {
            // Arrange
            var walletId = Guid.NewGuid();
            var transactions = new TransactionViewModelFaker().Generate(10);
            _transactionAppServiceMock.Setup(x => x.GetByWalletId(walletId)).ReturnsAsync(transactions);

            // Act
            var result = await _transactionController.GetByWalletId(walletId);

            // Assert
            var okObject = Assert.IsType<OkObjectResult>(result);
            var data = okObject.Value as SuccessResult<object>;
            Assert.NotNull(data);
            var returnedTransactions = Assert.IsAssignableFrom<List<TransactionViewModel>>(data.Data);
            Assert.Equal(10, returnedTransactions.Count);
        }

        [Fact]
        public async Task GetPaginatedTransactions_ShouldReturnAllPaginatedTransactionsFromWallet()
        {
            // Arrange
            int skip = 5, take = 5;
            var walletId = Guid.NewGuid();
            var transactions = new TransactionViewModelFaker().Generate(10);
            var paginatedList = new PaginatedSuccessResult<TransactionViewModel>(true, transactions, 10);
            _transactionAppServiceMock.Setup(x => x.GetAll(skip, take, walletId)).ReturnsAsync(paginatedList);

            // Act
            var result = await _transactionController.Pagination(skip, take, walletId);

            // Assert
            var okObject = Assert.IsType<OkObjectResult>(result);
            var data = okObject.Value as PaginatedSuccessResult<object>;
            Assert.NotNull(data);
            var returnedTransactions = Assert.IsAssignableFrom<List<TransactionViewModel>>(data.Data);
            Assert.Equal(10, returnedTransactions.Count);
        }

        [Fact]
        public async Task Post_ShouldReturnOkNoContent()
        {
            // Arrange
            var model = new CreateTransactionViewModel();
            _transactionAppServiceMock.Setup(x => x.Register(model)).Returns(Task.CompletedTask);

            // Act
            var result = await _transactionController.Post(model);

            // Assert
            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public async Task Post_ShouldReturnBadRequest()
        {
            // Arrange
            var model = new CreateTransactionViewModel();
            var notification = new DomainNotification("Name", "The Name field is required.");
            _transactionController.ModelState.AddModelError("Name", "The Name field is required.");
            _notificationHandlerMock.Setup(x => x.HasNotifications()).Returns(true);
            _notificationHandlerMock.Setup(x => x.GetNotifications()).Returns(new List<DomainNotification> { notification });
            _transactionAppServiceMock.Setup(x => x.Register(model)).Returns(Task.CompletedTask);

            // Act
            var result = await _transactionController.Post(model);

            // Assert
            var objectResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsType<ErrorResult<object>>(objectResult.Value);
            Assert.False(errorResult.Success);
            Assert.Equal(new[] { "The Name field is required." }, errorResult.Errors);
        }

        [Fact]
        public async Task History_ShouldReturnWalletHistory()
        {
            // Arrange
            var id = Guid.NewGuid();
            var history = new TransactionHistoryFaker().Generate(10);
            _transactionAppServiceMock.Setup(x => x.GetAllHistory(id)).ReturnsAsync(history);

            // Act
            var result = await _transactionController.History(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = okResult.Value as SuccessResult<object>;
            Assert.NotNull(data);
            var returnedHistory = Assert.IsAssignableFrom<List<TransactionHistoryData>>(data.Data);
            Assert.Equal(history.Count, returnedHistory.Count);
        }
    }
}
