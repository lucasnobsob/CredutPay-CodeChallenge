using CredutPay.Application.EventSourcedNormalizers;
using CredutPay.Application.Interfaces;
using CredutPay.Application.ViewModels;
using CredutPay.Domain.Core.Interfaces;
using CredutPay.Domain.Core.Notifications;
using CredutPay.Infra.CrossCutting.Identity.Authorization;
using CredutPay.Services.Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CredutPay.Services.API.Controllers
{
    [Authorize]
    public class WalletController : ApiController
    {
        private readonly IWalletAppService _walletAppService;
        private readonly ILogger<WalletController> _logger;

        public WalletController(
            ILogger<WalletController> logger,
            IWalletAppService walletAppService,
            INotificationHandler<DomainNotification> notifications,
            IMediatorHandler mediator) : base(notifications, mediator)
        {
            _walletAppService = walletAppService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WalletViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userId, out Guid result);
            var wallets = await _walletAppService.GetAll(result);
            return Response(wallets);
        }

        [HttpGet]
        [Route("user")]
        [ProducesResponseType(typeof(IEnumerable<WalletViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllByUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userId, out Guid result);
            var wallets = await _walletAppService.GetAllByUserId(result);
            return Response(wallets);
        }

        [HttpPost]
        [Authorize(Policy = "CanWriteWalletData", Roles = Roles.Admin)]
        [ProducesResponseType(typeof(CreateWalletViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CreateWalletViewModel createWalletViewModel)
        {
            _logger.LogInformation("Objeto recebido: {@walletViewModel}", createWalletViewModel);

            if (!ModelState.IsValid)
            {
                NotifyModelStateErrors();
                return Response(createWalletViewModel);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userId, out Guid result);
            createWalletViewModel.UserId = result;
            await _walletAppService.Register(createWalletViewModel);

            return Response(createWalletViewModel);
        }

        [HttpDelete]
        [Authorize(Policy = "CanRemoveWalletData", Roles = Roles.Admin)]
        [ProducesResponseType(typeof(WalletViewModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Guid recebido: {@guid}", id);

            await _walletAppService.Remove(id);

            return NoContent();
        }

        [HttpGet]
        [Route("history/{id:guid}")]
        [ProducesResponseType(typeof(IList<WalletHistoryData>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> History(Guid id)
        {
            var walletHistoryData = await _walletAppService.GetAllHistory(id);
            return Response(walletHistoryData);
        }
    }
}
