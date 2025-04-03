using CredutPay.Application.Interfaces;
using CredutPay.Application.Services;
using CredutPay.Domain.CommandHandlers;
using CredutPay.Domain.Commands;
using CredutPay.Domain.Core.Events;
using CredutPay.Domain.Core.Interfaces;
using CredutPay.Domain.Core.Notifications;
using CredutPay.Domain.EventHandlers;
using CredutPay.Domain.Events;
using CredutPay.Domain.Interfaces;
using CredutPay.Domain.Services;
using CredutPay.Infra.CrossCutting.Bus;
using CredutPay.Infra.CrossCutting.Identity.Authorization;
using CredutPay.Infra.CrossCutting.Identity.Models;
using CredutPay.Infra.CrossCutting.Identity.Services;
using CredutPay.Infra.Data.EventSourcing;
using CredutPay.Infra.Data.Repository;
using CredutPay.Infra.Data.Repository.EventSourcing;
using CredutPay.Infra.Data.UoW;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace CredutPay.Infra.CrossCutting.IoC
{
    public class NativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            // ASP.NET HttpContext dependency
            services.AddHttpContextAccessor();
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Domain Bus (Mediator)
            services.AddScoped<IMediatorHandler, InMemoryBus>();

            // ASP.NET Authorization Polices
            services.AddSingleton<IAuthorizationHandler, ClaimsRequirementHandler>();

            // Application
            services.AddScoped<IWalletAppService, WalletAppService>();
            services.AddScoped<ITransactionAppService, TransactionAppService>();

            // Domain - Events
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();
            services.AddScoped<INotificationHandler<TransactionRegisteredEvent>, TransactionEventHandler>();
            services.AddScoped<INotificationHandler<WalletRegisteredEvent>, WalletEventHandler>();
            services.AddScoped<INotificationHandler<WalletRemovedEvent>, WalletEventHandler>();

            // Domain - Commands
            services.AddScoped<IRequestHandler<RegisterNewTransactionCommand, bool>, TransactionCommandHandler>();
            services.AddScoped<IRequestHandler<RegisterNewWalletCommand, bool>, WalletCommandHandler>();
            services.AddScoped<IRequestHandler<RemoveWalletCommand, bool>, WalletCommandHandler>();

            // Domain - 3rd parties
            services.AddScoped<IHttpService, HttpService>();
            services.AddScoped<IMailService, MailService>();

            // Infra - Data
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Infra - Data EventSourcing
            services.AddScoped<IEventStoreRepository, EventStoreRepository>();
            services.AddScoped<IEventStore, SqlEventStore>();

            // Infra - Identity Services
            services.AddTransient<IEmailSender, AuthEmailMessageSender>();
            services.AddTransient<ISmsSender, AuthSMSMessageSender>();

            // Infra - Identity
            services.AddScoped<IUser, AspNetUser>();
            services.AddSingleton<IJwtFactory, JwtFactory>();
        }
    }
}
