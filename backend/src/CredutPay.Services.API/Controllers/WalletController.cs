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
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<WalletViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            var wallets = await _walletAppService.GetAll();
            return Response(wallets);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("{id:guid}")]
        [ProducesResponseType(typeof(WalletViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(Guid id)
        {
            _logger.LogInformation("Guid recebido: {@guid}", id);

            var wallet = await _walletAppService.GetById(id);

            return Response(wallet);
        }

        [HttpPost]
        //[Authorize(Policy = "CanWriteWalletData", Roles = Roles.Admin)]
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
        [AllowAnonymous]
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
