using CredutPay.Domain.Services;
using CredutPay.Services.API.Configurations;
using Polly;
using System.Text.Json;

namespace CredutPay.Services.Api.StartupExtensions
{
    public static class HttpExtension
    {
        public static IServiceCollection AddCustomizedHttp(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddHttpClient("Foo", c =>
                {
                    c.BaseAddress = new Uri(configuration.GetValue<string>("HttpClients:Foo"));
                })
                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(5, _ => TimeSpan.FromMilliseconds(500)))
                .AddTypedClient(c => Refit.RestService.For<IFooClient>(c));

            services.AddControllers(options =>
            {
                options.Conventions.Add(new LowercaseControllerRouteConvention());
            });

            return services;
        }
    }
}
