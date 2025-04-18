﻿using CredutPay.Application.EventSourcedNormalizers;
using CredutPay.Application.Interfaces;
using CredutPay.Application.ViewModels;
using CredutPay.Domain.Core.Interfaces;
using CredutPay.Domain.Core.Notifications;
using CredutPay.Infra.CrossCutting.Identity.Authorization;
using CredutPay.Services.Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CredutPay.Services.API.Controllers
{
    [Authorize]
    public class TransactionController : ApiController
    {
        private readonly ITransactionAppService _transactionAppService;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(
            INotificationHandler<DomainNotification> notifications,
            ITransactionAppService transactionAppService,
            ILogger<TransactionController> logger,
            IMediatorHandler mediator) : base(notifications, mediator)
        {
            _transactionAppService = transactionAppService;
            _logger = logger;
        }

        [HttpGet]
        [Route("wallet/{id:guid}")]
        [ProducesResponseType(typeof(TransactionViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByWalletId(Guid id)
        {
            _logger.LogInformation("Guid recebido: {@guid}", id);

            var transaction = await _transactionAppService.GetByWalletId(id);

            return Response(transaction);
        }

        [HttpPost]
        [Authorize(Policy = "CanWriteTransactionData", Roles = Roles.Admin)]
        [ProducesResponseType(typeof(TransactionViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CreateTransactionViewModel transactionCreateViewModel)
        {
            _logger.LogInformation("Objeto recebido: {@transactionViewModel}", transactionCreateViewModel);

            if (!ModelState.IsValid)
            {
                NotifyModelStateErrors();
                return Response(transactionCreateViewModel);
            }

            await _transactionAppService.Register(transactionCreateViewModel);

            return Created();
        }

        [HttpGet]
        [Route("history/{id:guid}")]
        [ProducesResponseType(typeof(IList<WalletHistoryData>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> History(Guid id)
        {
            var transactionHistoryData = await _transactionAppService.GetAllHistory(id);
            return Response(transactionHistoryData);
        }

        [HttpGet]
        [Route("wallet/pagination")]
        [ProducesResponseType(typeof(IEnumerable<TransactionViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Pagination(int skip, int take, Guid walletId)
        {
            var transactions = await _transactionAppService.GetAll(skip, take, walletId);
            return Response(transactions);
        }
    }
}
