using AutoMapper;
using CredutPay.Application.EventSourcedNormalizers;
using CredutPay.Application.Interfaces;
using CredutPay.Application.ViewModels;
using CredutPay.Domain.Core.Interfaces;
using CredutPay.Domain.Core.Notifications;
using CredutPay.Domain.Models;
using CredutPay.Services.API.Controllers;
using CredutPay.Tests.FakeData.Wallet;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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

        public WalletControllerTest()
        {
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
        }

        //[Fact]
        //public async Task GetAll_ShouldReturnAllWallets()
        //{
        //    // Arrange
        //    var wallets = new WalletViewModelFaker().Generate(10);
        //    _walletAppServiceMock.Setup(x => x.GetAllByUserId()).ReturnsAsync(wallets);

        //    // Act
        //    var result = await _walletController.GetAllByUserId();

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //    Assert.NotNull(okResult.Value);
        //    var dataProperty = okResult.Value.GetType().GetProperty("data");
        //    Assert.NotNull(dataProperty);
        //    var dataValue = dataProperty.GetValue(okResult.Value);
        //    var employeeList = Assert.IsType<List<WalletViewModel>>(dataValue);
        //    Assert.Equal(10, employeeList.Count);
        //}

    }
}
