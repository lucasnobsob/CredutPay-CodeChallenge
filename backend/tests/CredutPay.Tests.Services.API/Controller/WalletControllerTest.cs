using CredutPay.Application.EventSourcedNormalizers;
using CredutPay.Application.Interfaces;
using CredutPay.Application.ViewModels;
using CredutPay.Domain.Core.Interfaces;
using CredutPay.Domain.Core.Notifications;
using CredutPay.Services.API.Controllers;
using CredutPay.Tests.FakeData.Wallet;
using IdentityModel.OidcClient;
using k8s.KubeConfigModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace CredutPay.Tests.Services.API.Controller
{
    public class WalletControllerTest
    {
        private readonly Mock<IWalletAppService> _walletAppServiceMock;
        private readonly Mock<DomainNotificationHandler> _notificationHandlerMock;
        private readonly Mock<ILogger<WalletController>> _loggerMock;
        private readonly Mock<IMediatorHandler> _mediatorHandlerMock;
        private readonly WalletController _walletController;
        private readonly Guid _userId = Guid.NewGuid();

        public WalletControllerTest()
        {
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, _userId.ToString())
            }));
            _walletAppServiceMock = new Mock<IWalletAppService>();
            _notificationHandlerMock = new Mock<DomainNotificationHandler>();
            _loggerMock = new Mock<ILogger<WalletController>>();
            _mediatorHandlerMock = new Mock<IMediatorHandler>();

            _walletController = new WalletController(
                _loggerMock.Object,
                _walletAppServiceMock.Object,
                _notificationHandlerMock.Object,
                _mediatorHandlerMock.Object
            );

            _walletController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext
                {
                    User = userClaims
                }
            };
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllWallets()
        {
            // Arrange
            var wallets = new WalletViewModelFaker().Generate(10);
            _walletAppServiceMock.Setup(x => x.GetAll(_userId)).ReturnsAsync(wallets);

            // Act
            var result = await _walletController.GetAll();

            // Assert
            var okObject = Assert.IsType<OkObjectResult>(result);
            var data = okObject.Value as SuccessResult<object>;
            Assert.NotNull(data);
            var returnedWallets = Assert.IsAssignableFrom<List<WalletViewModel>>(data.Data);
            Assert.Equal(10, returnedWallets.Count);
        }

        [Fact]
        public async Task GetAllByUserId_ShouldReturnAllUserWallets()
        {
            // Arrange
            var wallets = new WalletViewModelFaker().Generate(10);
            _walletAppServiceMock.Setup(x => x.GetAllByUserId(_userId)).ReturnsAsync(wallets);

            // Act
            var result = await _walletController.GetAllByUserId();

            // Assert
            var okObject = Assert.IsType<OkObjectResult>(result);
            var data = okObject.Value as SuccessResult<object>;
            Assert.NotNull(data);
            var returnedWallets = Assert.IsAssignableFrom<List<WalletViewModel>>(data.Data);
            Assert.Equal(10, returnedWallets.Count);
        }

        [Fact]
        public async Task Post_ShouldReturnOkNoContent()
        {
            // Arrange
            var model = new CreateWalletViewModel();
            _walletAppServiceMock.Setup(x => x.Register(model)).Returns(Task.CompletedTask);

            // Act
            var result = await _walletController.Post(model);

            // Assert
            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public async Task Post_ShouldReturnBadRequest()
        {
            // Arrange
            var model = new CreateWalletViewModel();
            var notification = new DomainNotification("Name", "The Name field is required.");
            _walletController.ModelState.AddModelError( "Name", "The Name field is required.");
            _notificationHandlerMock.Setup(x => x.HasNotifications()).Returns(true);
            _notificationHandlerMock.Setup(x => x.GetNotifications()).Returns(new List<DomainNotification> { notification });
            _walletAppServiceMock.Setup(x => x.Register(model)).Returns(Task.CompletedTask);

            // Act
            var result = await _walletController.Post(model);

            // Assert
            var objectResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsType<ErrorResult<object>>(objectResult.Value);
            Assert.False(errorResult.Success);
            Assert.Equal(new[] { "The Name field is required." }, errorResult.Errors);
        }

        [Fact]
        public async Task Delete_ShouldReturnOkNoContent()
        {
            // Arrange
            var wallets = new WalletViewModelFaker().Generate(10);
            _walletAppServiceMock.Setup(x => x.Remove(_userId)).Returns(Task.CompletedTask);

            // Act
            var result = await _walletController.Delete(_userId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task History_ShouldReturnWalletHistory()
        {
            // Arrange
            var history = new WalletHistoryFaker().Generate(10);
            _walletAppServiceMock.Setup(x => x.GetAllHistory(_userId)).ReturnsAsync(history);

            // Act
            var result = await _walletController.History(_userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = okResult.Value as SuccessResult<object>;
            Assert.NotNull(data);
            var returnedHistory = Assert.IsAssignableFrom<List<WalletHistoryData>>(data.Data);
            Assert.Equal(history.Count, returnedHistory.Count);
        }
    }
}
